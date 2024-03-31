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
			var folderPath = "Assets/Editor/Resources";

			var basePath = string.Empty;
			var folders = folderPath.Split('/');

			for (var i = 1; i < folders.Length; i++)
			{
				basePath += folders[i - 1];
				AssetDatabase.CreateFolder(basePath, folders[i]);
				basePath += "/";
			}

			var settings = CreateInstance<Settings>();
			AssetDatabase.CreateAsset(settings, $"{folderPath}/{SettingsFilename}.asset");
			EditorUtility.SetDirty(settings);

			return settings;
		}

		public void OnAfterDeserialize()
		{
			Profiles.UpdateCache();
		}

		public void OnBeforeSerialize() { }
	}
}
