using System;
using System.Collections.Generic;
using UnityEditor;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal class ProfilesView
	{
		private const string NoneOption = "None";
		private const string SaveProfileOption = "Save Profile";
		private const string DeleteProfileOption = "Delete Profile";

		private readonly BitmapFontCreatorData _editorData;
		private readonly ProfileList _profiles;
		private string[] _options;
		private int _optionIndex = 0;

		public ProfilesView(BitmapFontCreatorData editorData, ProfileList profiles)
		{
			_editorData = editorData;
			_profiles = profiles;
			UpdateOptions();
			_optionIndex = _profiles.SelectedIndex + 1;
		}

		private void UpdateOptions()
		{
			var options = new List<string> { NoneOption };
			var names = _profiles.Names;
			foreach (var name in names)
				options.Add(name);
			options.Add(SaveProfileOption);
			options.Add(DeleteProfileOption);
			_options = options.ToArray();
		}

		public void Draw()
		{
			EditorGUI.BeginChangeCheck();
			var optionIndex = EditorGUILayout.Popup(UI.Profile, _optionIndex, _options);
			if (EditorGUI.EndChangeCheck()) SelectOption(optionIndex);
		}

		private void SelectOption(int index)
		{
			if (index < 0 || index >= _options.Length) return;

			if (index <= _profiles.Names.Length)
			{
				_optionIndex = index;
				_profiles.SelectedIndex = index - 1;
				_profiles.Selected?.CopyTo(_editorData);
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
			window.Open("Save Profile", _profiles.Selected?.Name ?? "Profile");
		}

		private void TrySaveOrAddProfile(string profileName)
		{
			var index = Array.IndexOf(_profiles.Names, profileName);
			if (index < 0)
			{
				// profile doesn't exist. Creating one and selecting it
				_profiles.Add(new Profile(profileName, _editorData));
				UpdateOptions();
				_optionIndex = _profiles.SelectedIndex + 1;
				return;
			}

			// profile does exit. Ask if it should be replaced
			if (EditorUtility.DisplayDialog("Profile Exists", "Profile already exists. Overwrite?", "Yes", "No"))
				_profiles.Update(_profiles.SelectedIndex, new Profile(profileName, _editorData));
		}

		private void DeleteSelectedProfile()
		{
			if (!EditorUtility.DisplayDialog("Delete Profile", "Are you sure you want to delete this profile?", "Yes", "No"))
				return;
			_profiles.RemoveAt(_profiles.SelectedIndex);
			_optionIndex = _profiles.SelectedIndex + 1;
			UpdateOptions();
		}
	}
}
