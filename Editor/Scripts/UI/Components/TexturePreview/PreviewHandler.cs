using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal class PreviewHandler
	{
		private static readonly Vector2Int _texturePadding = new(10, 10);
		private static readonly Vector2 _textureSizeRange = new(128f, 2048f);
		private static readonly float _gridTexSize = 48f;

		private Vector2 _scrollPos = Vector2.zero;

		// Info: This is a quick and dirty way to persist the zoom values between window opens and closes.
		// It assumes that there will be only one instance of this window at a time
		private static Vector2 _zoomRange = new(0.5f, 3f);
		private static float _zoom = 1f;

		public static void UpdateZoomRange(float width, float height)
		{
			var r = Mathf.Min(width, height);
			var min = _textureSizeRange.x / r;
			min = min < 1f
				? Mathf.Floor(min / 0.1f) * 0.1f             // rounding to the nearest 0.1
				: Mathf.Floor(min);                          // or flooring to the nearest integer
			var max = Mathf.Ceil(_textureSizeRange.y / r);  // ceiling to next integer

			if (min == _zoomRange.x && max == _zoomRange.y) return;
			_zoomRange = new Vector2(min, max);
			_zoom = Mathf.Clamp(1f, min, max);
		}

		public void DrawZoomSlider()
		{
			_zoom = EditorGUILayout.Slider(_zoom, _zoomRange.x, _zoomRange.y);
		}

		public void DrawTextureViewer(Rect position, ExecutionData data, PrefsModel prefs)
		{
			var e = Event.current;
			if (e.type == EventType.Repaint)
				EditorGUIUtility.AddCursorRect(position, _cursor);

			HandleTextureNavigation(position, e);
			DrawTexture(position, data, prefs);
		}

		private void DrawTexture(Rect position, ExecutionData data, PrefsModel prefs)
		{
			var tw = data.Texture.width;
			var th = data.Texture.height;

			var aw = position.width - _texturePadding.x * 2;
			var ah = position.height - _texturePadding.y * 2;

			var ratio = tw > th ? aw / tw : ah / th;

			var textureRect = new Rect(
				_texturePadding.x,
				_texturePadding.y + position.y,
				tw * _zoom,
				th * _zoom
			);

			var scrollContentRect = new Rect(
				0, position.y,
				textureRect.width + _texturePadding.x * 2,
				textureRect.height + _texturePadding.y * 2
			);

			_scrollPos = GUI.BeginScrollView(position, _scrollPos, scrollContentRect);
			GUI.DrawTextureWithTexCoords(textureRect,
				prefs.TextureBackground == 0 ? Styles.GridDarkTexture : Styles.GridLightTexture,
				new Rect(0, 0, textureRect.width / _gridTexSize, textureRect.height / _gridTexSize), true);
			GUI.DrawTexture(textureRect, data.Texture, ScaleMode.ScaleToFit, true);
			DrawGrid(textureRect, data, prefs, ratio);
			GUI.EndScrollView();
		}

		private void DrawGrid(Rect rect, ExecutionData data, PrefsModel prefs, float ratio)
		{
			var cellSize = new Vector2(rect.width / data.Cols, rect.height / data.Rows);

			var r = rect;
			for (var i = 0; i < data.Rows + 1; i++)
			{
				r.y = rect.y + i * cellSize.y;
				r.height = 1;
				EditorGUI.DrawRect(r, prefs.GridColor);
				if (i == 0) continue;

				r.height = 1;
				r.y -= data.Descent * ratio;
				EditorGUI.DrawRect(r, prefs.BaselineColor);

				r.y -= data.Ascent * ratio;
				EditorGUI.DrawRect(r, prefs.HeightColor);
			}

			r = rect;
			r.width = 1;
			for (var i = 0; i < data.Cols + 1; i++)
			{
				r.x = rect.x + i * cellSize.x;
				EditorGUI.DrawRect(r, prefs.GridColor);
			}
		}


		private MouseCursor _cursor = MouseCursor.Arrow;

		private void HandleTextureNavigation(Rect rect, Event e)
		{
			if (e.type == EventType.Repaint || e.type == EventType.Layout || !rect.Contains(e.mousePosition))
				return;

			_cursor = MouseCursor.Arrow;

			// zoom
			if (e.control)
			{
				_cursor = MouseCursor.Zoom;
				if (e.isScrollWheel)
				{
					_zoom = Mathf.Clamp(_zoom - e.delta.y, _zoomRange.x, _zoomRange.y);
					GUI.changed = true;
					e.Use();
				}
			}

			// pan
			if (e.button == 2 && e.type == EventType.MouseDrag)
			{
				_scrollPos.x -= e.delta.x;
				_scrollPos.y -= e.delta.y;
				_cursor = MouseCursor.Pan;
				e.Use();
			}
		}
	}
}