using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal class OptionsModel
	{
		private const string EditorPrefsKey = "Options";

		public bool WarnBeforeOverwrite = true;
		public bool WarnBeforeReplaceingSettings = true;

		public static OptionsModel Load()
		{
			var content = EditorPrefs.GetString(EditorPrefsKey, null);
			return string.IsNullOrEmpty(content)
				? new OptionsModel()
				: JsonUtility.FromJson<OptionsModel>(content);
		}

		public static void Save(OptionsModel model)
		{
			var content = JsonUtility.ToJson(model);
			EditorPrefs.SetString(EditorPrefsKey, content);
		}
	}
}