using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal static class UIUtils
	{
		public static float FloatFieldMin(GUIContent label, float value, float min)
		{
			EditorGUI.BeginChangeCheck();
			value = EditorGUILayout.FloatField(label, value);
			if (EditorGUI.EndChangeCheck() && value < min) value = min;
			return value;
		}

		public static int IntFieldMin(GUIContent label, int value, int min)
		{
			EditorGUI.BeginChangeCheck();
			value = EditorGUILayout.IntField(label, value);
			if (EditorGUI.EndChangeCheck() && value < min) value = min;
			return value;
		}

		public static float FloatFieldWithAuto(GUIContent label, float value, GUIContent autoLabel, ref bool auto)
		{
			GUILayout.BeginHorizontal();
			GUI.enabled = !auto;
			value = EditorGUILayout.FloatField(label, value);
			GUI.enabled = true;

			auto = EditorGUILayout.ToggleLeft(autoLabel, auto, GUILayout.Width(Styles.AutoToggleWidth));
			GUILayout.EndHorizontal();
			return value;
		}

		public static bool CenteredButton(GUIContent label, GUIStyle style = null)
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var value = GUILayout.Button(label, style ?? GUI.skin.button);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			return value;
		}
	}
}
