using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal class PrefsModel
	{
		private const string EditorPrefsKey = "dev.klebersilva.tools.bitmapfontcreator.preferences";

		public bool WarnOnReplaceFont = true;
		public bool WarnOnReplaceSettings = true;
		public bool WarnOnReplaceProfile = true;

		public static PrefsModel Load()
		{
			var content = EditorPrefs.GetString(EditorPrefsKey, null);
			return string.IsNullOrEmpty(content)
				? new PrefsModel()
				: JsonUtility.FromJson<PrefsModel>(content);
		}

		public static void Save(PrefsModel model)
		{
			var content = JsonUtility.ToJson(model);
			EditorPrefs.SetString(EditorPrefsKey, content);
		}
	}
}