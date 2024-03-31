using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace kleberswf.tools.bitmapfontcreator
{
	internal class ProfilesView
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

		public ProfilesView(BitmapFontCreatorData editorData)
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
				_prefs.SelectedProfile?.CopyTo(_editorData);
				return;
			}

			if (_options[index] == SaveProfileOption) ShowSaveDialog();
			if (_optionIndex > 0 && _options[index] == DeleteProfileOption)
				DeleteSelectedProfile();
		}

		private void ShowSaveDialog()
		{
			var window = EditorWindow.GetWindow<SaveDialog>();
			window.OnSave += TrySaveOrAddProfile;
			window.Open("Save Profile", _prefs.SelectedProfile?.Name ?? "Profile");
		}

		private void TrySaveOrAddProfile(string profileName)
		{
			var index = Array.IndexOf(_prefs.ProfileNames, profileName);
			if (index < 0)
			{
				// profile doesn't exist. Creating one and selecting it
				_prefs.AddProfile(new Profile(profileName, _editorData));
				UpdateOptions();
				_optionIndex = _prefs.SelectedProfileIndex + 1;
				return;
			}

			// profile does exit. Ask if it should be replaced
			if (EditorUtility.DisplayDialog("Profile Exists", "Profile already exists. Overwrite?", "Yes", "No"))
				_prefs.UpdateProfile(_prefs.SelectedProfileIndex, new Profile(profileName, _editorData));
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
}
