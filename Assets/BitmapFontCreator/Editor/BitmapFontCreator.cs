using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace kleberswf.tools.bitmapfontcreator
{
	internal static class BitmapFontCreator
	{
		public static void CreateFont(ExecutionData data)
		{
			var error = CheckForErrors(data);
			if (!string.IsNullOrEmpty(error))
			{
				Debug.LogError(error);
				return;
			}

			// TODO check if the asset already exists to warn the user

			var path = AssetDatabase.GetAssetPath(data.Texture);
			var baseName = Path.GetFileNameWithoutExtension(path);

			var material = CreateMaterial(baseName, data.Texture);
			var font = CreateFontAsset(baseName, material, data);

			path = path[..path.LastIndexOf(".")];
			AssetDatabase.CreateAsset(material, path + ".mat");
			CreateOrReplaceAsset(font, path + ".fontsettings");

			AssetDatabase.Refresh();
		}

		// TODO all checks
		private static string CheckForErrors(ExecutionData data)
		{
			if (data.Texture == null) return "Texture cannot be null";
			if (!data.Texture.isReadable) return "Texture must be readable. Set Read/Write Enabled to true inside Texture Properties";

			if (data.Characters.Length != data.Cols * data.Rows)
				return $"Characters length ({data.Characters.Length}) must be equal to Cols ({data.Cols}) * Rows ({data.Rows})";

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
			int xMin, yMin, xMax, yMax, advance;
			int largestAdvance = 0;

			// horizontal
			for (var row = 0; row < data.Rows; row++)
			{
				for (var col = 0; col < data.Cols; col++)
				{
					var i = data.Orientation == Orientation.Horizontal
						? (row * data.Cols) + col
						: (col * data.Rows) + row;
					var ch = data.Characters[i];
					if (ch == ' ' || ch == '\r' || ch == '\n') continue;

					GetCharacterBounds(
						data.Texture,
						data.AlphaThreshold,
						col * (int)cellSize.x,
						(data.Rows - row) * (int)cellSize.y,
						(int)cellSize.x,
						(int)cellSize.y,
						out xMin, out yMin, out xMax, out yMax
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

		// TODO maybe we can remove yMin and yMax
		private static void GetCharacterBounds(Texture2D tex, float alphaThreshold, int x0, int y0, int width, int height,
			out int xMin, out int yMin, out int xMax, out int yMax)
		{
			xMin = width;
			yMin = height;
			xMax = 0;
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
					if (x > xMax) xMax = x;
					if (y < yMin) yMin = y;
					if (y > yMax) yMax = y;
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
	}
}