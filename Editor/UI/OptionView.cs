using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal class OptionsView
	{
		private readonly OptionsModel _model;
		private Rect _buttonRect = Rect.zero;
		public OptionsModel Model => _model;

		public OptionsView(OptionsModel options) { _model = options; }

		public void Draw()
		{
			if (GUILayout.Button(UI.Options, Styles.OptionsButton)) PopupWindow.Show(_buttonRect, new OptionsPopup(_model));
			if (Event.current.type == EventType.Repaint) _buttonRect = GUILayoutUtility.GetLastRect();
		}
	}

	internal class OptionsPopup : PopupWindowContent
	{
		private readonly OptionsModel _model;
		private static readonly RectOffset _padding = new(8, 8, 6, 8);
		private static readonly Vector2 _size = new(190 + _padding.horizontal, EditorGUIUtility.singleLineHeight * 2 + _padding.vertical);

		public OptionsPopup(OptionsModel model) : base() { _model = model; }

		public override Vector2 GetWindowSize() => _size;

		public override void OnGUI(Rect rect)
		{
			rect.height = EditorGUIUtility.singleLineHeight;
			rect.x += _padding.left;
			rect.y += _padding.right;
			_model.WarnBeforeOverwrite = EditorGUI.ToggleLeft(rect, UI.WarnBeforeOverwrite, _model.WarnBeforeOverwrite);
			rect.y += rect.height;
			_model.WarnBeforeReplaceingSettings = EditorGUI.ToggleLeft(rect, UI.WarnBeforeReplaceingSettings, _model.WarnBeforeReplaceingSettings);
		}

		public override void OnClose()
		{
			base.OnClose();
			OptionsModel.Save(_model);
		}
	}
}