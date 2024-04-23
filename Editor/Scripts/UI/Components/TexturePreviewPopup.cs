using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal class TexturePreviewPopup : EditorWindow
	{
		private static readonly Vector2Int _texturePadding = new(20, 20);
		private static readonly Vector2 _textureSizeRange = new(128f, 2048f);
		private static readonly float _gridTexSize = 48f;

		// Info: This is a quick and dirty way to persist the zoom values between window opens and closes.
		// It assumes that there will be only one instance of this window at a time
		private static Vector2 _zoomRange = new(0.5f, 3f);
		private static float _zoom = 1f;

		private ExecutionData _data;
		private PrefsModel _prefs;
		private bool _hasContent;

		private Rect _contentRect = new();
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
			_hasContent = _data != null && _data.Texture != null && _data.Rows > 0 && _data.Cols > 0;

			if (_hasContent)
				UpdateZoomRange(_data.Texture.width, _data.Texture.height);
			ShowUtility();
		}

		private void UpdateZoomRange(float width, float height)
		{
			var r = Mathf.Min(width, height);
			var min = _textureSizeRange.x / r;
			min = min < 1f
				? Mathf.Floor(min / 0.1f) * 0.1f // rounding to the nearest 0.1
				: Mathf.Floor(min);              // or flooring to the nearest integer
			var max = Mathf.Ceil(_textureSizeRange.y / r);  // ceiling to next integer

			if (min == _zoomRange.x && max == _zoomRange.y) return;
			_zoomRange = new Vector2(min, max);
			_zoom = Mathf.Clamp(1f, min, max);
		}

		private void OnGUI()
		{
			if (EditorApplication.isPlaying)
			{
				Close();
				return;
			}

			if (_hasContent) DrawContent();
			else GUILayout.Label(UIContent.NoContent, Styles.CenterLabel);
		}

		private void DrawContent()
		{
			DrawTopBar();
			var y = GUILayoutUtility.GetLastRect().yMax;
			_contentRect = new Rect(0, y, position.width, position.height - y);
			TextureNavigationHandler.HandleTextureNavigation(_contentRect, _zoomRange, ref _zoom, ref _scrollPos);
			DrawTexture();
		}

		private void DrawTopBar()
		{
			GUILayout.BeginHorizontal(Styles.Toolbar);
			GUILayout.FlexibleSpace();

			DrawZoomGroup();

			UIUtils.Divider(10, 10);
			DrawColorGroup();

			UIUtils.Divider(0, 5);
			DrawBackgroundSelector();
			GUILayout.Space(10);

			GUILayout.EndHorizontal();
		}

		private void DrawZoomGroup()
		{
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.Label(UIContent.ZoomLabel, Styles.ToolbarLabel);
			EditorGUIUtility.labelWidth = 30;
			EditorGUIUtility.fieldWidth = 30;
			_zoom = EditorGUILayout.Slider(_zoom, _zoomRange.x, _zoomRange.y);
			GUILayout.EndHorizontal();
		}

		private void DrawColorGroup()
		{
			GUILayout.Label(UIContent.ColorsLabel, Styles.ToolbarLabel);
			_prefs.GridColor = UIUtils.ColorField(UIContent.GridColorLabel, _prefs.GridColor);
			_prefs.HeightColor = UIUtils.ColorField(UIContent.HeightColorLabel, _prefs.HeightColor);
			_prefs.BaselineColor = UIUtils.ColorField(UIContent.BaselineColorLabel, _prefs.BaselineColor);
		}

		private void DrawBackgroundSelector()
		{
			var selection = GUILayout.Toggle(
				_prefs.TextureBackground == 0,
				_prefs.TextureBackground == 0 ? UIContent.DarkTextureIcon : UIContent.LightTextureIcon,
				Styles.BackgroundTextureIcon);
			_prefs.TextureBackground = selection ? 0 : 1;
		}

		private void DrawTexture()
		{
			var tw = _data.Texture.width;
			var th = _data.Texture.height;

			var aw = position.width - _texturePadding.x * 2;
			var ah = position.height - _texturePadding.y * 2;

			var ratio = tw > th ? aw / tw : ah / th;

			var textureRect = new Rect(
				_texturePadding.x,
				_texturePadding.y + _contentRect.y,
				tw * _zoom,
				th * _zoom
			);

			var scrollContentRect = new Rect(
				0, _contentRect.y,
				textureRect.width + _texturePadding.x * 2,
				textureRect.height + _texturePadding.y * 2
			);

			_scrollPos = GUI.BeginScrollView(_contentRect, _scrollPos, scrollContentRect);
			GUI.DrawTextureWithTexCoords(textureRect,
				_prefs.TextureBackground == 0 ? Styles.GridDarkTexture : Styles.GridLightTexture,
				new Rect(0, 0, textureRect.width / _gridTexSize, textureRect.height / _gridTexSize), true);
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
