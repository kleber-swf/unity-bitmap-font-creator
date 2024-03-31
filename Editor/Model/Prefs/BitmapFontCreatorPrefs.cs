using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal class BitmapFontCreatorPrefs : ScriptableObject, ISerializationCallbackReceiver
	{
		private const string PrefsFilename = "BitmapFontCreatorPrefs";

		public ProfileList Profiles;

		public static BitmapFontCreatorPrefs Load()
		{
			var prefs = Resources.Load(PrefsFilename, typeof(BitmapFontCreatorPrefs)) as BitmapFontCreatorPrefs;
			if (prefs == null) prefs = CreateAsset();
			return prefs;
		}

		private static BitmapFontCreatorPrefs CreateAsset()
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

			var prefs = CreateInstance<BitmapFontCreatorPrefs>();
			AssetDatabase.CreateAsset(prefs, $"{folderPath}/{PrefsFilename}.asset");
			EditorUtility.SetDirty(prefs);

			return prefs;
		}

		public void OnAfterDeserialize()
		{
			Profiles.UpdateCache();
		}

		public void OnBeforeSerialize() { }
	}
}
