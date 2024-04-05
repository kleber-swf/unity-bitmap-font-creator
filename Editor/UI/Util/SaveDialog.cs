using System;
using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal class SaveDialog : EditorWindow
	{
		private const string TextFieldId = "_tf";
		private string _text = "";
		public event Action<string> OnSave;

		public static SaveDialog Open(string title, string text)
		{
			var window = GetWindowWithRect<SaveDialog>(
				new Rect(Screen.width * 0.5f, Screen.height * 0.5f, 300, 60), false, title ?? "Save");
			window.DoOpen(text);
			return window;
		}

		private void DoOpen(string text)
		{
			_text = text ?? "";
			minSize = maxSize = new Vector2(300, 60);
			ShowModalUtility();
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
			Close();
			if (OnSave == null) return;
			OnSave.Invoke(_text);
			foreach (var handler in OnSave.GetInvocationList())
				OnSave -= (Action<string>)handler;
		}
	}
}
