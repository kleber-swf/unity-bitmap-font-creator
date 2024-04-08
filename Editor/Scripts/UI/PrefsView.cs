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
			if (GUILayout.Button(UI.PreferencesButton, Styles.PreferencesButton)) PopupWindow.Show(_buttonRect, new PrefsPopup(_model));
			if (Event.current.type == EventType.Repaint) _buttonRect = GUILayoutUtility.GetLastRect();
		}
	}

	internal class PrefsPopup : PopupWindowContent
	{
		private readonly PrefsModel _model;
		private static readonly RectOffset _padding = new(8, 8, 6, 8);
		private const int _headerHeight = 26;

		private static readonly Vector2 _size = new(
			200 + _padding.horizontal,
			EditorGUIUtility.singleLineHeight * 3 + _padding.vertical + _headerHeight
		);

		public PrefsPopup(PrefsModel model) : base() { _model = model; }

		public override Vector2 GetWindowSize() => _size;

		public override void OnGUI(Rect rect)
		{
			rect.height = _headerHeight;
			GUI.Label(rect, UI.PreferencesLabel, Styles.SectionLabel);

			rect.x += _padding.left;
			rect.y += _padding.top + rect.height;
			rect.height = EditorGUIUtility.singleLineHeight;
			_model.WarnOnReplaceFont = EditorGUI.ToggleLeft(rect, UI.WarnOnReplaceFont, _model.WarnOnReplaceFont);

			rect.y += rect.height;
			_model.WarnOnReplaceSettings = EditorGUI.ToggleLeft(rect, UI.WarnOnReplaceSettings, _model.WarnOnReplaceSettings);

			rect.y += rect.height;
			_model.WarnOnReplaceProfile = EditorGUI.ToggleLeft(rect, UI.WarnOnReplaceProfile, _model.WarnOnReplaceProfile);
		}

		public override void OnClose()
		{
			base.OnClose();
			PrefsModel.Save(_model);
		}
	}
}