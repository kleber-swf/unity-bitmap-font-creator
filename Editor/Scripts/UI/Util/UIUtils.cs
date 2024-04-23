using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal static class UIUtils
	{
		public static int IntFieldMin(GUIContent label, int value, int min)
		{
			EditorGUI.BeginChangeCheck();
			value = EditorGUILayout.IntField(label, value);
			if (EditorGUI.EndChangeCheck() && value < min) value = min;
			return value;
		}

		public static int IntFieldWithAuto(GUIContent label, int value, GUIContent autoLabel, ref bool auto)
		{
			GUILayout.BeginHorizontal();
			GUI.enabled = !auto;
			value = EditorGUILayout.IntField(label, value);
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

		public static Color ColorField(GUIContent label, Color value)
		{
			value = EditorGUILayout.ColorField(GUIContent.none, value, false, true, false, GUILayout.Width(Styles.ColorFieldWidth));
			GUILayout.Label(label, Styles.ToolbarLabel);
			return value;
		}

		public static void Divider(int left = 0, int right = 0)
		{
			GUILayout.Space(left);
			GUILayout.Label(GUIContent.none, Styles.Divider);
			GUILayout.Space(right);
		}
	}
}
