using UnityEditor;
using UnityEngine;

namespace kleberswf.tools.bitmapfontcreator
{
	internal class BitmapFontCreatorPrefs : ScriptableObject, ISerializationCallbackReceiver
	{
		private const string PrefsFilename = "BitmapFontCreatorPrefs";
		private const string PrefsFilepath = "Assets/BitmapFontCreator/Resources/" + PrefsFilename + ".asset";

		public ProfileList Profiles;

		public static BitmapFontCreatorPrefs Load()
		{
			var prefs = Resources.Load(PrefsFilename, typeof(BitmapFontCreatorPrefs)) as BitmapFontCreatorPrefs;
			if (prefs == null)
			{
				prefs = CreateInstance<BitmapFontCreatorPrefs>();
				AssetDatabase.CreateAsset(prefs, PrefsFilepath);
				EditorUtility.SetDirty(prefs);
			}
			return prefs;
		}

		public void OnAfterDeserialize()
		{
			Profiles.UpdateCache();
		}

		public void OnBeforeSerialize() { }
	}
}
