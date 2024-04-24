using UnityEngine;
using UnityEditor;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal class TexturePreviewPopup : EditorWindow
	{
		private readonly PreviewHandler _preview = new();

		private ExecutionData _data;
		private PrefsModel _prefs;
		private bool _hasContent;

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
				PreviewHandler.UpdateZoomRange(_data.Texture.width, _data.Texture.height);
			ShowUtility();
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
			_preview.DrawTextureViewer(position, y, _data, _prefs);
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
			EditorGUIUtility.labelWidth = 30;
			EditorGUIUtility.fieldWidth = 30;
			GUILayout.Label(UIContent.ZoomLabel, Styles.ToolbarLabel);
			_preview.DrawZoomSlider();
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

		private void OnDisable()
		{
			if (_prefs != null) PrefsModel.Save(_prefs);
		}
	}
}
