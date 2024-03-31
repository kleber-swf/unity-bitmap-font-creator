using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace kleberswf.tools.bitmapfontcreator
{
	[Serializable]
	internal class Profile : BitmapFontCreatorData
	{
		public string Name;
		public Profile() { }

		public Profile(string name, BitmapFontCreatorData src)
		{
			Name = name;
			src?.CopyTo(this);
		}
	}

	internal class BitmapFontCreatorPrefs : ScriptableObject
	{
		[SerializeField] private int _selectedProfileIndex = -1;
		[SerializeField] private List<Profile> _profiles = new();

		private string[] _profileNames;

		public int SelectedProfileIndex
		{
			get => _selectedProfileIndex;
			set => _selectedProfileIndex = value;
		}

		public Profile SelectedProfile
		{
			get
			{
				return _selectedProfileIndex < 0 || _selectedProfileIndex >= _profiles.Count
					? null : _profiles[_selectedProfileIndex];
			}
		}

		public string[] ProfileNames => _profileNames;

		public void CreateCache()
		{
			var lastSelected = _selectedProfileIndex < 0 ? null : _profiles[_selectedProfileIndex].Name;
			_profiles.Sort((a, b) => a.Name.CompareTo(b.Name));
			_profileNames = _profiles.Select(e => e.Name).ToArray();
			if (!string.IsNullOrEmpty(lastSelected)) _selectedProfileIndex = Array.IndexOf(_profileNames, lastSelected);
		}

		public void AddProfile(Profile data)
		{
			_profiles.Add(data);
			_selectedProfileIndex = _profiles.Count - 1;
			CreateCache();
		}

		public bool RemoveProfileAt(int index)
		{
			if (index < 0 || index >= _profiles.Count) return false;
			_profiles.RemoveAt(index);
			_selectedProfileIndex = -1;
			CreateCache();
			return true;
		}

		public void UpdateProfile(int index, Profile data)
		{
			if (data == null) return;
			if (index < 0 || index >= _profiles.Count)
				AddProfile(data);
			else
				data.CopyTo(_profiles[index]);
		}
	}
}
