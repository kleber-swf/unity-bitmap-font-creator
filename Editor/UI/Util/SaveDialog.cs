using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal class SaveDialog : EditorWindow
	{
		private const string TextFieldId = "_tf";
		private string _text = "";
		private bool _save = false;

		public static string Open(string title, string text)
		{
			var window = GetWindowWithRect<SaveDialog>(
				new Rect(Screen.width * 0.5f, Screen.height * 0.5f, 300, 60), false, title ?? "Save");
			return window.DoOpen(text);
		}

		private string DoOpen(string text)
		{
			_text = text ?? string.Empty;
			minSize = maxSize = new Vector2(300, 60);
			ShowModalUtility();
			return _save ? _text : null;
		}

		private void OnGUI()
		{
			GUILayout.BeginVertical();
			GUI.SetNextControlName(TextFieldId);
			_text = EditorGUILayout.TextField(_text);
			if (GUILayout.Button("Save")) RequestSaveAndClose();
			GUILayout.EndVertical();
			EditorGUI.FocusTextInControl(TextFieldId);
		}

		private void RequestSaveAndClose()
		{
			_save = true;
			Close();
		}
	}
}
