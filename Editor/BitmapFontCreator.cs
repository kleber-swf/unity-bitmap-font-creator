using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal static class BitmapFontCreator
	{
		private struct FontMeasures
		{
			public float MaxCharHeight;
			public Vector2Int cellSize;
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

			var measures = new FontMeasures();
			var font = new Font(baseName)
			{
				material = material,
				characterInfo = CreateCharacters(data, map, data.Descent, ref measures),
			};

			var so = new SerializedObject(font);
			so.FindProperty("m_LineSpacing").floatValue = CalcLineSpacing(data, measures);
			so.FindProperty("m_Ascent").floatValue = data.Ascent + data.Descent;
			so.FindProperty("m_Descent").floatValue = data.Descent;
			so.FindProperty("m_FontSize").floatValue = CalcFontSize(data, measures);
			so.ApplyModifiedProperties();

			return font;
		}

		private static CharacterInfo[] CreateCharacters(ExecutionData data, Dictionary<char, CharacterProps> map,
			float descent, ref FontMeasures measures)
		{
			var texSize = new Vector2Int(data.Texture.width, data.Texture.height);
			var cellSize = new Vector2Int(Mathf.FloorToInt(texSize.x / data.Cols), Mathf.FloorToInt(texSize.y / data.Rows));
			var cellUVSize = new Vector2(1f / data.Cols, 1f / data.Rows);
			var ratio = new Vector2(1f / texSize.x, 1f / texSize.y);

			var characters = new List<CharacterInfo>();
			int xMin, xMax, yMin, yMax, advance, y, h, advMax = 0;

			var baseline = (int)(cellSize.y - descent);
			measures.cellSize = cellSize;

			for (var row = 0; row < data.Rows; row++)
			{
				for (var col = 0; col < data.Cols; col++)
				{
					var i = data.Orientation == Orientation.Horizontal
						? (row * data.Cols) + col
						: (col * data.Rows) + row;
					var ch = data.ValidCharacters[i];
					if (ch == IgnoreCharacter) continue;

					GetCharacterBounds(
						tex: data.Texture,
						alphaThreshold: data.AlphaThreshold,
						x0: col * cellSize.x,
						y0: (data.Rows - row) * cellSize.y,
						width: cellSize.x,
						height: cellSize.y,
						xMin: out xMin,
						xMax: out xMax,
						yMin: out yMin,
						yMax: out yMax
					);

					advance = xMax - xMin + data.DefaultCharacterSpacing;
					if (advance > advMax) advMax = advance;

					y = Mathf.RoundToInt(-yMin + descent - (yMax - baseline));
					h = yMax - yMin;
					if (h > measures.MaxCharHeight) measures.MaxCharHeight = h;

					var info = new CharacterInfo
					{
						index = ch,
						uvTopLeft = new Vector2(
							cellUVSize.x * col + (xMin * ratio.x),
							cellUVSize.y * (data.Rows - row) - (yMin * ratio.y)
						),
						uvBottomRight = new Vector2(
							cellUVSize.x * (col + 1) - ((cellSize.x - xMax) * ratio.x),
							cellUVSize.y * (data.Rows - row - 1) + ((cellSize.y - yMax) * ratio.y)
						),
						minX = xMin,
						maxX = xMax,
						minY = yMin + y,
						maxY = yMax + y,
						bearing = 0,
						advance = advance,
					};

					if (map.TryGetValue(ch, out var props))
					{
						info.minX += props.Padding.x;
						info.maxX += props.Padding.x;
						info.advance += props.Padding.x + props.Spacing;
						info.minY -= props.Padding.y;
						info.maxY -= props.Padding.y;
					}

					characters.Add(info);
				}
			}

			if (data.Monospaced)
				SetFontAsMonospaced(characters, advMax);

			return characters.ToArray();
		}

		private static void GetCharacterBounds(Texture2D tex, float alphaThreshold, int x0, int y0,
			int width, int height, out int xMin, out int xMax, out int yMin, out int yMax)
		{
			xMin = width;
			xMax = 0;
			yMin = height;
			yMax = 0;

			int xx, yy;
			for (var y = 0; y < height; y++)
			{
				yy = y0 - y;
				for (var x = 0; x < width; x++)
				{
					xx = x0 + x;
					if (tex.GetPixel(xx, yy).a <= alphaThreshold) continue;
					if (x < xMin) xMin = x;
					if (x + 1 > xMax) xMax = x + 1;
					if (y < yMin) yMin = y;
					if (y + 1 > yMax) yMax = y + 1;
				}
			}
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

		private static float CalcLineSpacing(ExecutionData data, FontMeasures measures)
		{
			if (!data.AutoLineSpacing) return data.LineSpacing;
			if (measures.MaxCharHeight > 0) return measures.MaxCharHeight;
			return measures.cellSize.y;
		}

		private static float CalcFontSize(ExecutionData data, FontMeasures measures)
		{
			if (!data.AutoFontSize) return data.FontSize;
			if (data.Ascent > 0) return data.Ascent;
			if (measures.MaxCharHeight > 0) return measures.MaxCharHeight;
			return measures.cellSize.y;
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
