using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	[Serializable]
	internal class ProfileList
	{
		[SerializeField] private int _selectedIndex = -1;
		[SerializeField] private List<Profile> _profiles = new();
		private string[] _names = new string[0];

		[NonSerialized] public Settings Parent;

		public int SelectedIndex
		{
			get => _selectedIndex;
			set
			{
				_selectedIndex = value;
				UpdateAsset();
			}
		}

		public Profile Selected
		{
			get
			{
				return _selectedIndex < 0 || _selectedIndex >= _profiles.Count
					? null : _profiles[_selectedIndex];
			}
		}

		public string[] Names => _names;

		public void Add(Profile data)
		{
			_profiles.Add(data);
			SelectedIndex = _profiles.Count - 1;
			UpdateCache();
		}

		public bool RemoveAt(int index)
		{
			if (index < 0 || index >= _profiles.Count) return false;
			_profiles.RemoveAt(index);
			SelectedIndex = -1;
			UpdateCache();
			return true;
		}

		public void Update(int index, Profile data)
		{
			if (data == null) return;
			if (index < 0 || index >= _profiles.Count)
				Add(data);
			else
			{
				data.CopyTo(_profiles[index]);
				SelectedIndex = index;
			}
		}

		public void UpdateCache()
		{
			var lastSelected = _selectedIndex < 0 ? null : _profiles[_selectedIndex].Name;
			_profiles.Sort((a, b) => a.Name.CompareTo(b.Name));
			_names = _profiles.Select(e => e.Name).ToArray();
			if (!string.IsNullOrEmpty(lastSelected)) _selectedIndex = Array.IndexOf(_names, lastSelected);
		}

		private void UpdateAsset()
		{
			if (!Parent) return;
			EditorUtility.SetDirty(Parent);
			AssetDatabase.SaveAssetIfDirty(Parent);
		}
	}
}