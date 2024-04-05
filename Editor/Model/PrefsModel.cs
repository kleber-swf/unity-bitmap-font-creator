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
		public Color BackgroundColor = new(0.1764706f, 0.238087f, 0.3960784f);
		public Color GridColor = new(0.1066661f, 0.1220122f, 0.1603774f, 1f);

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