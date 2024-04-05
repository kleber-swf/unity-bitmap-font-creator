using UnityEngine;
using UnityEditor;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	public class TexturePreviewPopup : EditorWindow
	{
		private static readonly Vector2Int _texturePadding = new(20, 20);

		private Texture2D _texture;
		private int _rows;
		private int _cols;
		private bool _hasContent;

		private Vector2 _scrollPos = Vector2.zero;

		public static TexturePreviewPopup Open(Texture2D texture, int rows, int cols)
		{
			var window = GetWindow<TexturePreviewPopup>();
			window.DoOpen(texture, rows, cols);
			return window;
		}

		private void DoOpen(Texture2D texture, int rows, int cols)
		{
			minSize = new Vector2(400, 300);
			maxSize = new Vector2(1920, 1920);
			_texture = texture;
			_rows = rows;
			_cols = cols;
			_hasContent = _texture != null && _rows > 0 && _cols > 0;
			ShowModal();
		}

		private void OnGUI()
		{
			if (_hasContent) DrawTexture();
			else DrawNoContent();
		}

		private void DrawTexture()
		{
			var tw = _texture.width;
			var th = _texture.height;

			var aw = position.width - _texturePadding.x * 2;
			var ah = position.height - _texturePadding.y * 2;

			var ratio = tw > th ? aw / tw : ah / th;

			var textureRect = new Rect(
				_texturePadding.x,
				_texturePadding.y,
				tw * ratio,
				th * ratio
			);

			var scrollRect = new Rect(0, 0, position.width, position.height);
			var scrollContentRect = new Rect(0, 0, textureRect.width + _texturePadding.x * 2, textureRect.height + _texturePadding.y * 2);

			_scrollPos = GUI.BeginScrollView(scrollRect, _scrollPos, scrollContentRect);
			GUI.DrawTexture(textureRect, _texture, ScaleMode.ScaleToFit, false);
			DrawGrid(textureRect);
			GUI.EndScrollView();
		}

		private void DrawNoContent()
		{
			GUILayout.Label(UI.NoContent, Styles.CenterLabel);
		}

		private void DrawGrid(Rect rect)
		{

			var cellSize = new Vector2(rect.width / _cols, rect.height / _rows);

			var r = rect;
			r.height = 1;
			for (var i = 0; i < _rows + 1; i++)
			{
				r.y = rect.y + i * cellSize.y;
				EditorGUI.DrawRect(r, Color.cyan);
			}

			r = rect;
			r.width = 1;
			for (var i = 0; i < _cols + 1; i++)
			{
				r.x = rect.x + i * cellSize.x;
				EditorGUI.DrawRect(r, Color.cyan);
			}
		}
	}
}
