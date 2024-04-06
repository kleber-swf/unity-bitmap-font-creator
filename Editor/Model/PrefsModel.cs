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
		public int TextureBackground = 0;
		public Color GridColor = new(0.6509434f, 0.6509434f, 0.6509434f, 1f);
		public Color BaselineColor = new(1f, 0.4692699f, 0f, 0.65f);
		public Color HeightColor = new(0f, 0.8527439f, 1f, 0.5f);

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