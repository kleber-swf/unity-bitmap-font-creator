using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	public partial class BitmapFontCreatorEditor
	{
		private void DrawTextureGroup()
		{
			EditorGUILayout.BeginVertical(Styles.Group);

			DrawTextureField();
			_data.Cols = UIUtils.IntFieldMin(UIContent.Cols, _data.Cols, 1);
			_data.Rows = UIUtils.IntFieldMin(UIContent.Rows, _data.Rows, 1);

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(UIContent.GuessRowsAndColsButton, Styles.MiniButton)) GuessRowsAndCols();
			_showPreview = DrawPreviewButton();
			GUILayout.EndHorizontal();

			EditorGUILayout.Space();
			_data.Orientation = (Orientation)EditorGUILayout.EnumPopup(UIContent.Orientation, _data.Orientation);
			_data.AlphaThreshold = EditorGUILayout.Slider(UIContent.AlphaThreshold, _data.AlphaThreshold, 0f, 1f);

			EditorGUILayout.EndVertical();
		}

		private void DrawTextureField()
		{
			EditorGUI.BeginChangeCheck();
			_data.Texture = EditorGUILayout.ObjectField(UIContent.TextureLabel, _data.Texture, typeof(Texture2D), false) as Texture2D;
			if (EditorGUI.EndChangeCheck()) _guessRowsColsCache = Vector2Int.zero;
		}

		private bool DrawPreviewButton()
		{
			GUI.enabled = _data.Texture != null && _data.Rows > 0 && _data.Cols > 0;
			var value = GUILayout.Button(UIContent.PreviewButton, Styles.MiniButton);
			GUI.enabled = true;
			return value;
		}

		private void GuessRowsAndCols()
		{
			if (_data.Texture == null)
			{
				Debug.LogWarning("Texture cannot be null");
				return;
			}

			_guessRowsColsCache = _guessRowsColsCache.x == 0 || _guessRowsColsCache.y == 0
				? BitmapFontCreator.GuessRowsAndCols(_data.Texture)
				: _guessRowsColsCache;

			_data.Cols = _guessRowsColsCache.y;
			_data.Rows = _guessRowsColsCache.x;
		}
	}
}
