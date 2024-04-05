using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal class Settings : ScriptableObject, ISerializationCallbackReceiver
	{
		private const string SettingsFilename = "BitmapFontCreatorSettings";

		public ProfileList Profiles;

		public static Settings Load()
		{
			var settings = Resources.Load(SettingsFilename, typeof(Settings)) as Settings;
			if (settings == null) settings = CreateAsset();
			return settings;
		}

		private static Settings CreateAsset()
		{
			var folderPath = "Editor/Resources";
			var folders = folderPath.Split("/");
			var path = Application.dataPath;

			for (var i = 0; i < folders.Length; i++)
			{
				path = Path.Combine(path, folders[i]);
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
			}

			AssetDatabase.Refresh();

			var settings = CreateInstance<Settings>();
			AssetDatabase.CreateAsset(settings, $"Assets/{folderPath}/{SettingsFilename}.asset");
			EditorUtility.SetDirty(settings);

			return settings;
		}

		public void OnAfterDeserialize()
		{
			Profiles.Parent = this;
			Profiles.UpdateCache();
		}

		public void OnBeforeSerialize() { }
	}
}
