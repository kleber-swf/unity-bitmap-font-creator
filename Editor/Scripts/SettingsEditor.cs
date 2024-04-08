using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	[CustomEditor(typeof(Settings))]
	public class SettingsEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			GUILayout.Label(
				"You cannot edit this file in the inspector. Please use the menu: "
				+ BitmapFontCreatorEditor.MenuItemPath
				+ " or the following button.",
				EditorStyles.helpBox
			);
			EditorGUILayout.Space();
			if (GUILayout.Button("Open Bitmap Font Creator"))
				BitmapFontCreatorEditor.ShowWindow();
		}
	}
}