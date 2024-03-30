using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace kleberswf.tools.bitmapfontcreator
{
	internal class ProfilesUI
	{
		private const string PrefsFilename = "BitmapFontCreatorPrefs";
		private const string PrefsFilepath = "Assets/BitmapFontCreator/Resources/" + PrefsFilename + ".asset";
		private const string NoneOption = "None";
		private const string SaveProfileOption = "Save Profile";
		private const string DeleteProfileOption = "Delete Profile";

		private readonly BitmapFontCreatorData _editorData;
		private readonly BitmapFontCreatorPrefs _prefs;
		private string[] _options;
		private int _optionIndex = 0;

		public ProfilesUI(BitmapFontCreatorData editorData)
		{
			_editorData = editorData;
			_prefs = Resources.Load(PrefsFilename, typeof(BitmapFontCreatorPrefs)) as BitmapFontCreatorPrefs;
			if (_prefs == null)
			{
				_prefs = ScriptableObject.CreateInstance<BitmapFontCreatorPrefs>();
				AssetDatabase.CreateAsset(_prefs, PrefsFilepath);
				EditorUtility.SetDirty(_prefs);
			}
			_prefs.CreateCache();
			UpdateOptions();
			_optionIndex = _prefs.SelectedProfileIndex + 1;
		}

		private void UpdateOptions()
		{
			var options = new List<string> { NoneOption };
			var names = _prefs.ProfileNames;
			foreach (var name in names)
				options.Add(name);
			options.Add(SaveProfileOption);
			options.Add(DeleteProfileOption);
			_options = options.ToArray();
		}

		public void Draw()
		{
			GUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();

			var optionIndex = EditorGUILayout.Popup(UI.Profile, _optionIndex, _options);
			if (EditorGUI.EndChangeCheck()) SelectOption(optionIndex);
			GUILayout.EndHorizontal();
		}

		private void SelectOption(int index)
		{
			if (index < 0 || index >= _options.Length) return;

			if (index <= _prefs.ProfileNames.Length)
			{
				_optionIndex = index;
				_prefs.SelectedProfileIndex = index - 1;
				ProfileController.LoadProfile(_editorData, _prefs.SelectedProfile);
				return;
			}

			if (_options[index] == SaveProfileOption) ShowSaveDialog();
			if (_optionIndex > 0 && _options[index] == DeleteProfileOption)
				DeleteSelectedProfile();
		}

		private void ShowSaveDialog()
		{
			var window = EditorWindow.GetWindow<InputWindow>();
			window.OnClose += TrySaveProfile;
			window.Open(_prefs.SelectedProfile?.Name);
		}

		private void TrySaveProfile(string profileName)
		{
			var index = Array.IndexOf(_prefs.ProfileNames, profileName);
			if (index < 0)
			{
				_prefs.AddProfile(ProfileController.CreateProfile(_editorData, profileName));
				UpdateOptions();
				_optionIndex = _prefs.SelectedProfileIndex + 1;
				return;
			}

			if (EditorUtility.DisplayDialog("Profile Exists", "Profile already exists. Overwrite?", "Yes", "No"))
				_prefs.UpdateProfile(_prefs.SelectedProfileIndex, ProfileController.CreateProfile(_editorData, profileName));
		}

		private void DeleteSelectedProfile()
		{
			if (EditorUtility.DisplayDialog("Delete Profile", "Are you sure you want to delete this profile?", "Yes", "No"))
			{
				_prefs.RemoveProfileAt(_prefs.SelectedProfileIndex);
				_optionIndex = _prefs.SelectedProfileIndex + 1;
				UpdateOptions();
			}
		}

	}

	internal static class ProfileController
	{
		public static void LoadProfile(BitmapFontCreatorData editorData, Profile profile)
		{
			if (profile != null) editorData.CopyFrom(profile);
		}

		public static Profile CreateProfile(BitmapFontCreatorData editorData, string name)
		{
			var profile = new Profile() { Name = name };
			profile.CopyFrom(editorData);
			return profile;
		}
	}

	internal class InputWindow : EditorWindow
	{
		private string _text = "";
		public event Action<string> OnClose;

		public void Open(string text)
		{
			titleContent = new GUIContent("Save Profile");
			_text = text ?? "Profile";
		}

		private void OnGUI()
		{
			GUILayout.BeginVertical();
			_text = EditorGUILayout.TextField(_text);
			if (GUILayout.Button("Save")) DoClose();
			GUILayout.EndVertical();
		}

		private void DoClose()
		{
			Close();
			OnClose?.Invoke(_text);
		}
	}
}
