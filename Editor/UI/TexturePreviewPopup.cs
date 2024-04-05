using UnityEngine;
using UnityEditor;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal class TexturePreviewPopup : EditorWindow
	{
		private static readonly Vector2Int _texturePadding = new(20, 20);

		private ExecutionData _data;
		private PrefsModel _prefs;

		private Vector2 _scrollPos = Vector2.zero;

		public static TexturePreviewPopup Open(ExecutionData data, PrefsModel prefs)
		{
			var window = GetWindow<TexturePreviewPopup>(true, "Font Texture Preview", true);
			window.DoOpen(data, prefs);
			return window;
		}

		private void DoOpen(ExecutionData data, PrefsModel prefs)
		{
			minSize = new Vector2(400, 300);
			maxSize = new Vector2(1920, 1920);
			_data = data;
			_prefs = prefs;
			ShowUtility();
		}

		private void OnGUI()
		{
			if (_data.Texture != null && _data.Rows > 0 && _data.Cols > 0) DrawContent();
			else GUILayout.Label(UI.NoContent, Styles.CenterLabel);
		}

		private void DrawContent()
		{
			DrawTopBar();
			DrawTexture(GUILayoutUtility.GetLastRect().yMax);
		}

		private void DrawTopBar()
		{
			EditorGUIUtility.fieldWidth = 60;
			GUILayout.BeginHorizontal(Styles.Toolbar);
			GUILayout.FlexibleSpace();
			EditorGUIUtility.labelWidth = 80;
			_prefs.BackgroundColor = EditorGUILayout.ColorField(UI.BackgroundColor, _prefs.BackgroundColor);
			GUILayout.Space(10);
			EditorGUIUtility.labelWidth = 35;
			_prefs.GridColor = EditorGUILayout.ColorField(UI.GridColor, _prefs.GridColor);
			GUILayout.EndHorizontal();
		}

		private void DrawTexture(float y)
		{
			var tw = _data.Texture.width;
			var th = _data.Texture.height;

			var aw = position.width - _texturePadding.x * 2;
			var ah = position.height - _texturePadding.y * 2;

			var ratio = tw > th ? aw / tw : ah / th;

			var textureRect = new Rect(
				_texturePadding.x,
				_texturePadding.y + y,
				tw * ratio,
				th * ratio
			);

			var scrollRect = new Rect(0, y, position.width, position.height);
			var scrollContentRect = new Rect(0, y, textureRect.width + _texturePadding.x * 2, textureRect.height + _texturePadding.y * 2 + y);

			_scrollPos = GUI.BeginScrollView(scrollRect, _scrollPos, scrollContentRect);
			EditorGUI.DrawRect(textureRect, _prefs.BackgroundColor);
			GUI.DrawTexture(textureRect, _data.Texture, ScaleMode.ScaleToFit, true);
			DrawGrid(textureRect);
			GUI.EndScrollView();
		}

		private void DrawGrid(Rect rect)
		{
			var cellSize = new Vector2(rect.width / _data.Cols, rect.height / _data.Rows);

			var r = rect;
			r.height = 1;
			for (var i = 0; i < _data.Rows + 1; i++)
			{
				r.y = rect.y + i * cellSize.y;
				EditorGUI.DrawRect(r, _prefs.GridColor);
			}

			r = rect;
			r.width = 1;
			for (var i = 0; i < _data.Cols + 1; i++)
			{
				r.x = rect.x + i * cellSize.x;
				EditorGUI.DrawRect(r, _prefs.GridColor);
			}
		}

		private void OnDisable()
		{
			if (_prefs != null) PrefsModel.Save(_prefs);
		}
	}
}
