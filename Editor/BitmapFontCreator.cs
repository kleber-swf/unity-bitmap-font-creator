using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal static class BitmapFontCreator
	{
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
			var material = new Material(Shader.Find("Standard"))
			{
				name = baseName,
				mainTexture = texture,
			};

			// TextMesh support from https://github.com/Unity-Technologies/UnityCsReference/blob/e3365924358684e2c5d99ce1de1068bea5483981/Editor/Mono/Inspector/StandardShaderGUI.cs#L369
			material.SetOverrideTag("RenderType", "TransparentCutout");
			material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
			material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
			material.SetFloat("_ZWrite", 1.0f);
			material.EnableKeyword("_ALPHATEST_ON");
			material.DisableKeyword("_ALPHABLEND_ON");
			material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			// material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;

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

			return new Font(baseName)
			{
				material = material,
				characterInfo = CreateCharacters(data, map),
			};
		}

		private static CharacterInfo[] CreateCharacters(ExecutionData data, Dictionary<char, CharacterProps> map)
		{
			var texSize = new Vector2Int(data.Texture.width, data.Texture.height);
			var cellSize = new Vector2Int(Mathf.FloorToInt(texSize.x / data.Cols), Mathf.FloorToInt(texSize.y / data.Rows));
			var cellUVSize = new Vector2(1f / data.Cols, 1f / data.Rows);

			var characters = new List<CharacterInfo>();
			int xMin, xMax, advance, w, advMax = 0;

#if BITMAP_FONT_CREATOR_DEBUG
			static string _(float x) => $"<color=yellow>{x}</color>";
#endif

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
						xMax: out xMax
					);

					w = xMax - xMin;

					advance = w + data.DefaultCharacterSpacing;
					if (map.TryGetValue(ch, out var props)) advance = xMax + props.Spacing;
					if (advance > advMax) advMax = advance;

					var info = new CharacterInfo
					{
						index = ch,
						uvTopLeft = new Vector2(cellUVSize.x * col, cellUVSize.y * (data.Rows - row - 1)),
						uvBottomRight = new Vector2(cellUVSize.x * (col + 1), cellUVSize.y * (data.Rows - row)),
						minX = 0,
						maxX = Mathf.RoundToInt(cellSize.x),
						minY = Mathf.RoundToInt(cellSize.y * 0.5f),
						maxY = Mathf.RoundToInt(-cellSize.y * 0.5f),
						bearing = -xMin,
						advance = advance,
					};
					characters.Add(info);

#if BITMAP_FONT_CREATOR_DEBUG
					Debug.Log($"<b>{ch}</b> xMin: {_(xMin)} xMax: {_(xMax)} advance: {_(advance)}");
#endif
				}
			}

			if (data.Monospaced)
			{
				for (var i = 0; i < characters.Count; i++)
				{
					var c = characters[i];
					var minX = c.minX + Mathf.FloorToInt((advMax - c.advance) * 0.5f);
					c.minX = 0;
					c.maxX = cellSize.x;
					c.bearing = minX;
					c.advance = advMax;
					characters[i] = c;
				}
			}

			return characters.ToArray();
		}

		private static void GetCharacterBounds(Texture2D tex, float alphaThreshold, int x0, int y0,
			int width, int height, out int xMin, out int xMax)
		{
			xMin = width;
			xMax = 0;
			// yMin = height;
			// yMax = 0;

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
					// if (y < yMin) yMin = y;
					// if (y > yMax) yMax = y;
				}
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
	}
}
