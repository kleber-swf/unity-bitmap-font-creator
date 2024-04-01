using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal static class BitmapFontCreator
	{
		private const char IgnoreCharacter = ' ';

		public static void TryCreateFont(ExecutionData data, bool warnBeforeOverwrite)
		{
			var error = CheckForErrors(data);
			if (!string.IsNullOrEmpty(error))
			{
				Debug.LogError(error);
				return;
			}

			var path = AssetDatabase.GetAssetPath(data.Texture);
			var baseName = Path.GetFileNameWithoutExtension(path);
			path = path[..path.LastIndexOf(".")];
			var materialPath = path + ".mat";
			var fontPath = path + ".fontsettings";

			if (warnBeforeOverwrite && !(AssetDatabase.GUIDFromAssetPath(materialPath) == null && AssetDatabase.GUIDFromAssetPath(fontPath) == null))
			{
				if (!EditorUtility.DisplayDialog("Warning", "Asset already exists. Overwrite? (It will keep the references)", "Yes", "No"))
					return;
			}

			var material = CreateMaterial(baseName, data.Texture);
			var font = CreateFontAsset(baseName, material, data);

			AssetDatabase.CreateAsset(material, materialPath);
			CreateOrReplaceAsset(font, fontPath);

			AssetDatabase.Refresh();
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
			return new Material(Shader.Find("Standard"))
			{
				name = baseName,
				mainTexture = texture
			};
		}

		private static Font CreateFontAsset(string baseName, Material material, ExecutionData data)
		{
			var map = new Dictionary<char, CharacterProps>();
			foreach (var e in data.CustomCharacterProps)
				map.Add(e.Character[0], e);

			return new Font(baseName)
			{
				material = material,
				characterInfo = CreateCharacters(data, map),
			};
		}

		private static CharacterInfo[] CreateCharacters(ExecutionData data, Dictionary<char, CharacterProps> map)
		{
			var texSize = new Vector2(data.Texture.width, data.Texture.height);
			var cellSize = new Vector2(texSize.x / data.Cols, texSize.y / data.Rows);
			var cellUVSize = new Vector2(1f / data.Cols, 1f / data.Rows);

			var characters = new List<CharacterInfo>();
			int xMin, xMax, advance;
			int largestAdvance = 0;

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
						x0: col * (int)cellSize.x,
						y0: (data.Rows - row) * (int)cellSize.y,
						width: (int)cellSize.x,
						height: (int)cellSize.y,
						xMin: out xMin,
						xMax: out xMax
					);

					advance = xMax - xMin + data.DefaultCharacterSpacing;
					if (advance > largestAdvance) largestAdvance = advance;
					if (map.TryGetValue(ch, out var props)) advance = xMax - xMin + props.Spacing;

					var info = new CharacterInfo
					{
						index = ch,
						uvTopLeft = new Vector2(cellUVSize.x * col, cellUVSize.y * (data.Rows - row - 1)),
						uvBottomRight = new Vector2(cellUVSize.x * (col + 1), cellUVSize.y * (data.Rows - row)),
						minX = 0,
						minY = Mathf.RoundToInt(cellSize.y * 0.5f),
						maxX = Mathf.RoundToInt(cellSize.x),
						maxY = Mathf.RoundToInt(-cellSize.y * 0.5f),
						bearing = xMin,
						advance = advance,
					};
					characters.Add(info);
				}
			}

			if (data.Monospaced)
			{
				for (var i = 0; i < characters.Count; i++)
				{
					var c = characters[i];
					if (map.ContainsKey((char)c.index)) continue;
					c.advance = largestAdvance;
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
			T existingAsset = AssetDatabase.LoadAssetAtPath<T>(path);
			if (existingAsset == null)
				AssetDatabase.CreateAsset(asset, path);
			else
			{
				EditorUtility.CopySerialized(asset, existingAsset);
				AssetDatabase.SaveAssets();
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
