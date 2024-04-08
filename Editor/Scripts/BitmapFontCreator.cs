using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal static class BitmapFontCreator
	{
		private struct FontMetrics
		{
			public float MaxCharHeight;
			public Vector2Int cellSize;
		}

		private struct GlyphInfo
		{
			public float uvTop;
			public float uvLeft;
			public float uvBottom;
			public float uvRight;
			public int xMin;
			public int xMax;
			public int yMin;
			public int yMax;
			public int advance;
			public int y;
			public int h;
		}

		private const char IgnoreCharacter = ' ';

		public static bool TryCreateFont(ExecutionData data, bool warnBeforeOverwrite, out string error)
		{
			error = CheckForErrors(data);
			if (!string.IsNullOrEmpty(error)) return false;

			var path = AssetDatabase.GetAssetPath(data.Texture);
			var baseName = Path.GetFileNameWithoutExtension(path);
			path = path[..path.LastIndexOf(".")];
			var materialPath = path + ".mat";
			var fontPath = path + ".fontsettings";

			if (warnBeforeOverwrite && !(AssetDatabase.GUIDFromAssetPath(materialPath) == null && AssetDatabase.GUIDFromAssetPath(fontPath) == null))
			{
				if (!EditorUtility.DisplayDialog("Warning", "Asset already exists. Overwrite? (It will keep the references)", "Yes", "No"))
					return false;
			}

			var material = CreateMaterial(baseName, data.Texture);
			var font = CreateFontAsset(baseName, material, data, out error);
			if (!string.IsNullOrEmpty(error)) return false;

			AssetDatabase.CreateAsset(material, materialPath);
			CreateOrReplaceAsset(font, fontPath);

			AssetDatabase.Refresh();
			return true;
		}

		private static string CheckForErrors(ExecutionData data)
		{
			if (data.Cols < 1) return "Cols must be greater than 0";
			if (data.Rows < 1) return "Rows must be greater than 0";

			if (data.Texture == null) return "Texture cannot be null";
			if (!data.Texture.isReadable) return "Texture must be readable. Set Read/Write Enabled to true inside Texture Properties";

			if (data.ValidCharactersCount != data.Cols * data.Rows)
				return $"Characters length ({data.ValidCharactersCount}) must be equal to Cols ({data.Cols}) * Rows ({data.Rows})";

			return null;
		}

		private static Material CreateMaterial(string baseName, Texture2D texture)
		{
			var material = new Material(Shader.Find("Unlit/Transparent"))
			{
				name = baseName,
				mainTexture = texture,
			};

			return material;
		}

		private static Font CreateFontAsset(string baseName, Material material, ExecutionData data, out string error)
		{
			error = null;
			var map = new Dictionary<char, CharacterProps>();
			for (var i = 0; i < data.CustomCharacterProps.Count; i++)
			{
				var e = data.CustomCharacterProps[i];
				if (string.IsNullOrEmpty(e.Character))
				{
					error = $"Character for Custom Character Properties at position {i + 1} is empty";
					return null;
				}
				map.Add(e.Character[0], e);
			}

			var metrics = new FontMetrics();
			var font = new Font(baseName)
			{
				material = material,
				characterInfo = CreateCharacters(data, map, data.Descent, ref metrics),
			};

			var so = new SerializedObject(font);
			so.FindProperty("m_LineSpacing").floatValue = CalcLineSpacing(data, metrics);
			so.FindProperty("m_Ascent").floatValue = data.Ascent + data.Descent;
			so.FindProperty("m_Descent").floatValue = data.Descent;
			so.FindProperty("m_FontSize").floatValue = CalcFontSize(data, metrics);
			so.ApplyModifiedProperties();

			return font;
		}

		private static CharacterInfo[] CreateCharacters(ExecutionData data, Dictionary<char, CharacterProps> map,
			float descent, ref FontMetrics metrics)
		{
			var texSize = new Vector2Int(data.Texture.width, data.Texture.height);
			var cellSize = new Vector2Int(Mathf.FloorToInt(texSize.x / data.Cols), Mathf.FloorToInt(texSize.y / data.Rows));
			var cellUVSize = new Vector2(1f / data.Cols, 1f / data.Rows);
			var ratio = new Vector2(1f / texSize.x, 1f / texSize.y);

			var characters = new List<CharacterInfo>();
			int advanceMax = 0, advanceMin = int.MaxValue;

			var baseline = (int)(cellSize.y - descent);
			metrics.cellSize = cellSize;

			for (var row = 0; row < data.Rows; row++)
			{
				for (var col = 0; col < data.Cols; col++)
				{
					var i = data.Orientation == Orientation.Horizontal
						? (row * data.Cols) + col
						: (col * data.Rows) + row;
					var ch = data.ValidCharacters[i];
					if (ch == IgnoreCharacter) continue;

					var gi = new GlyphInfo();

					GetCharacterBounds(
						tex: data.Texture,
						alphaThreshold: data.AlphaThreshold,
						x0: col * cellSize.x,
						y0: ((data.Rows - row) * cellSize.y) - 1,
						width: cellSize.x,
						height: cellSize.y,
						ref gi
					);

					gi.advance = gi.xMax - gi.xMin + data.DefaultCharacterSpacing;
					if (gi.advance > advanceMax) advanceMax = gi.advance;
					if (gi.advance < advanceMin) advanceMin = gi.advance;

					gi.y = Mathf.RoundToInt(-gi.yMin + descent - (gi.yMax - baseline));
					gi.h = gi.yMax - gi.yMin;
					if (gi.h > metrics.MaxCharHeight) metrics.MaxCharHeight = gi.h;

					gi.uvTop = cellUVSize.x * col + (gi.xMin * ratio.x);
					gi.uvLeft = cellUVSize.y * (data.Rows - row) - (gi.yMin * ratio.y);
					gi.uvBottom = cellUVSize.x * (col + 1) - ((cellSize.x - gi.xMax) * ratio.x);
					gi.uvRight = cellUVSize.y * (data.Rows - row - 1) + ((cellSize.y - gi.yMax) * ratio.y);

					var info = CreateCharacterInfo(ch, gi, map);
					characters.Add(info);

					if (!data.CaseInsentive) continue;
					var ch2 = char.ToUpper(ch);
					if (ch2 != ch) characters.Add(CreateCharacterInfo(ch2, gi, map));
					ch2 = char.ToLower(ch2);
					if (ch2 != ch) characters.Add(CreateCharacterInfo(ch2, gi, map));

#if BITMAP_FONT_CREATOR_DEV
					static string _(float x) => $"<color=yellow>{x}</color>";
					Debug.Log($"<b>{ch}</b> w: {_(info.glyphWidth)} h: {_(info.glyphHeight)} yMin: {_(gi.yMin)} yMax: {_(gi.yMax)}");
#endif
				}
			}

			characters.Add(CreateSpaceCharacter(advanceMin));

			if (data.Monospaced)
				SetFontAsMonospaced(characters, advanceMax);

			return characters.ToArray();
		}

		private static CharacterInfo CreateCharacterInfo(char ch, GlyphInfo g, Dictionary<char, CharacterProps> map)
		{
			var info = new CharacterInfo
			{
				index = ch,
				uvTopLeft = new(g.uvTop, g.uvLeft),
				uvBottomRight = new(g.uvBottom, g.uvRight),
				minX = g.xMin,
				maxX = g.xMax,
				minY = g.yMin + g.y,
				maxY = g.yMax + g.y,
				bearing = 0,
				advance = g.advance,
			};

			if (map.TryGetValue(ch, out var props))
			{
				info.minX += props.Padding.x;
				info.maxX += props.Padding.x;
				info.advance += props.Padding.x + props.Spacing;
				info.minY -= props.Padding.y;
				info.maxY -= props.Padding.y;
			}
			return info;
		}

		private static void GetCharacterBounds(Texture2D tex, float alphaThreshold, int x0, int y0,
			int width, int height, ref GlyphInfo info)
		{
			var xMin = width;
			var xMax = 0;
			var yMin = height;
			var yMax = 0;

			int xx, yy;
			for (var y = 0; y < height; y++)
			{
				yy = y0 - y;
				for (var x = 0; x < width; x++)
				{
					xx = x0 + x;
					if (tex.GetPixel(xx, yy).a <= alphaThreshold) continue;
					if (x < xMin) xMin = x;
					if (x > xMax) xMax = x;
					if (y < yMin) yMin = y;
					if (y > yMax) yMax = y;
				}
			}
			info.xMin = xMin;
			info.xMax = xMax + 1;
			info.yMin = yMin;
			info.yMax = yMax + 1;
		}

		private static CharacterInfo CreateSpaceCharacter(int advance)
		{
			return new CharacterInfo
			{
				index = ' ',
				minX = 0,
				maxX = 0,
				minY = 0,
				maxY = 0,
				bearing = 0,
				advance = advance
			};
		}

		private static void SetFontAsMonospaced(List<CharacterInfo> characters, int advMax)
		{
			for (var i = 0; i < characters.Count; i++)
			{
				var c = characters[i];
				var x = c.minX + Mathf.RoundToInt((advMax - c.advance) * 0.5f);
				c.minX += x;
				c.maxX += x;
				c.advance = advMax;
				characters[i] = c;
			}
		}

		private static void CreateOrReplaceAsset<T>(T asset, string path) where T : Object
		{
			T dest = AssetDatabase.LoadAssetAtPath<T>(path);
			if (dest == null)
				AssetDatabase.CreateAsset(asset, path);
			else
			{
				EditorUtility.CopySerialized(asset, dest);
				EditorUtility.SetDirty(dest);
				AssetDatabase.SaveAssetIfDirty(dest);
			}
		}

		private static float CalcLineSpacing(ExecutionData data, FontMetrics metrics)
		{
			if (!data.AutoLineSpacing) return data.LineSpacing;
			if (metrics.MaxCharHeight > 0) return metrics.MaxCharHeight;
			return metrics.cellSize.y;
		}

		private static float CalcFontSize(ExecutionData data, FontMetrics metrics)
		{
			if (!data.AutoFontSize) return data.FontSize;
			if (data.Ascent > 0) return data.Ascent;
			if (metrics.MaxCharHeight > 0) return metrics.MaxCharHeight;
			return metrics.cellSize.y;
		}

		public static Vector2Int GuessRowsAndCols(Texture2D tex)
		{
			var rows = 0;
			var cols = 0;

			uint state = 0;   // 0 = looking for not transparent, 1 = looking for transparent
			bool foundNonTransparentPixel;

			for (var x = 0; x < tex.width; x++)
			{
				foundNonTransparentPixel = false;
				for (var y = 0; y < tex.height; y++)
				{
					if (tex.GetPixel(x, y).a == 0) continue;
					foundNonTransparentPixel = true;
					break;
				}

				if (state == 0)
				{
					if (foundNonTransparentPixel)
					{
						state = 1;
						cols++;
					}
				}
				else
				{
					if (!foundNonTransparentPixel)
						state = 0;
				}
			}

			state = 0;
			for (var y = 0; y < tex.height; y++)
			{
				foundNonTransparentPixel = false;
				for (var x = 0; x < tex.width; x++)
				{
					if (tex.GetPixel(x, y).a == 0) continue;
					foundNonTransparentPixel = true;
					break;
				}

				if (state == 0)
				{
					if (foundNonTransparentPixel)
					{
						state = 1;
						rows++;
					}
				}
				else
				{
					if (!foundNonTransparentPixel)
						state = 0;
				}
			}

			return new(rows, cols);
		}

		public static int GuessLineSpacing(Texture2D texture, int rows)
		{
			return Mathf.RoundToInt(texture.height / rows);
		}
	}
}
