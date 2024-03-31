using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal class PrefsView
	{
		private readonly PrefsModel _model;
		private Rect _buttonRect = Rect.zero;
		public PrefsModel Model => _model;

		public PrefsView(PrefsModel prefs) { _model = prefs; }

		public void Draw()
		{
			if (GUILayout.Button(UI.Prefs, Styles.PrefsButton)) PopupWindow.Show(_buttonRect, new PrefsPopup(_model));
			if (Event.current.type == EventType.Repaint) _buttonRect = GUILayoutUtility.GetLastRect();
		}
	}

	internal class PrefsPopup : PopupWindowContent
	{
		private readonly PrefsModel _model;
		private static readonly RectOffset _padding = new(8, 8, 6, 8);
		private static readonly Vector2 _size = new(190 + _padding.horizontal, EditorGUIUtility.singleLineHeight * 2 + _padding.vertical);

		public PrefsPopup(PrefsModel model) : base() { _model = model; }

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
			PrefsModel.Save(_model);
		}
	}
}