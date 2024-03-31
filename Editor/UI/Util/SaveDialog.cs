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

		public void Open(string title, string text)
		{
			titleContent = new GUIContent(title ?? "Save");
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
			OnSave?.Invoke(_text);
		}
	}
}
