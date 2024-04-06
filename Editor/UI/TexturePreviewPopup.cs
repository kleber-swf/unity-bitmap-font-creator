using UnityEngine;
using UnityEditor;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal class TexturePreviewPopup : EditorWindow
	{
		private static readonly Vector2Int _texturePadding = new(20, 20);
		private static readonly Vector2 _gridTexSize = new(48f, 48f);

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
			GUILayout.BeginHorizontal(Styles.Toolbar);
			GUILayout.FlexibleSpace();

			GUILayout.Space(5);
			GUILayout.Label(UI.GridColorLabel);
			_prefs.GridColor = EditorGUILayout.ColorField(GUIContent.none, _prefs.GridColor, false, true, false, GUILayout.Width(24));

			GUILayout.Space(5);
			GUILayout.Label(UI.HeightColorLabel);
			_prefs.HeightColor = EditorGUILayout.ColorField(GUIContent.none, _prefs.HeightColor, false, true, false, GUILayout.Width(24));

			GUILayout.Space(5);
			GUILayout.Label(UI.BaselineColorLabel);
			_prefs.BaselineColor = EditorGUILayout.ColorField(GUIContent.none, _prefs.BaselineColor, false, true, false, GUILayout.Width(24));

			GUILayout.Space(10);
			var selection = GUILayout.Toggle(
				_prefs.TextureBackground == 0,
				_prefs.TextureBackground == 0 ? UI.DarkTextureIcon : UI.LightTextureIcon,
				Styles.BackgroundTextureIcon);
			_prefs.TextureBackground = selection ? 0 : 1;

			GUILayout.EndHorizontal();
		}

		private void DrawTexture(float y)
		{
			var tw = _data.Texture.width;
			var th = _data.Texture.height;

			var aw = position.width - _texturePadding.x * 2;
			var ah = position.height - _texturePadding.y * 2;

			var ratio = tw > th ? aw / tw : ah / th;

			var textureRect = new Rect(_texturePadding.x,
				_texturePadding.y + y,
				tw * ratio,
				th * ratio
			);

			var scrollRect = new Rect(0, y, position.width, position.height);
			var scrollContentRect = new Rect(0, y, textureRect.width + _texturePadding.x * 2, textureRect.height + _texturePadding.y * 2 + y);

			_scrollPos = GUI.BeginScrollView(scrollRect, _scrollPos, scrollContentRect);
			GUI.DrawTextureWithTexCoords(textureRect,
				_prefs.TextureBackground == 0 ? UI.GridDarkTexture : UI.GridLightTexture,
				new Rect(0, 0, tw / _gridTexSize.x, th / _gridTexSize.y), true);
			GUI.DrawTexture(textureRect, _data.Texture, ScaleMode.ScaleToFit, true);
			DrawGrid(textureRect, _data.Ascent, _data.Descent, ratio);
			GUI.EndScrollView();
		}

		private void DrawGrid(Rect rect, float ascent, float descent, float ratio)
		{
			var cellSize = new Vector2(rect.width / _data.Cols, rect.height / _data.Rows);

			var r = rect;
			for (var i = 0; i < _data.Rows + 1; i++)
			{
				r.y = rect.y + i * cellSize.y;
				r.height = 1;
				EditorGUI.DrawRect(r, _prefs.GridColor);
				if (i == 0) continue;

				r.height = 1;
				r.y -= descent * ratio;
				EditorGUI.DrawRect(r, _prefs.BaselineColor);

				r.y -= ascent * ratio;
				EditorGUI.DrawRect(r, _prefs.HeightColor);
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
