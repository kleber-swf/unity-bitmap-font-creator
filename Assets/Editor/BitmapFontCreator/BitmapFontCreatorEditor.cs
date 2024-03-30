using UnityEngine;
using UnityEditor;

namespace kleberswf.tools.bitmapfontcreator
{
	// TODO save last configuration
	// TODO pre configured profiles
	// TODO save profiles to the project
	// TODO no error on play
	public class BitmapFontCreatorEditor : EditorWindow
	{
		private BitmapFontCreatorData _data = BitmapFontCreatorData.Default;
		private CharacterPropsList _customCharPropsList;
		private GUIStyle _buttonStyle;

		private bool _initialized = false;
		private Vector2 _scrollPos = Vector2.zero;

		[MenuItem("Window/Bitmap Font Creator")]
		private static void ShowWindow()
		{
			var window = GetWindow<BitmapFontCreatorEditor>();
			window.titleContent = new GUIContent("Bitmap Font Creator");
			window.minSize = new Vector2(300, 500);
			window.Show();
		}

		private void Initialize()
		{
			_initialized = true;
			_customCharPropsList = new CharacterPropsList(_data.CustomCharacterProps);

			_buttonStyle = new(EditorStyles.miniButton)
			{
				fontSize = 12,
				padding = new RectOffset(20, 20, 8, 8),
				fixedHeight = 32,
			};
		}

		private void OnGUI()
		{
			if (_customCharPropsList == null) Initialize();

			GUILayout.BeginVertical();

			_data.Texture = EditorGUILayout.ObjectField("Font Texture", _data.Texture, typeof(Texture2D), false) as Texture2D;
			_data.Orientation = (Orientation)EditorGUILayout.EnumPopup("Orientation", _data.Orientation);

			_data.Cols = EditorGUILayout.IntField("Cols", _data.Cols);
			_data.Rows = EditorGUILayout.IntField("Rows", _data.Rows);
			_data.AlphaThreshold = EditorGUILayout.FloatField("Alpha Threshold", _data.AlphaThreshold);
			_data.Monospaced = EditorGUILayout.Toggle("Monospaced", _data.Monospaced);
			_data.LineSpacing = EditorGUILayout.IntField("Line Spacing", _data.LineSpacing);

			EditorGUILayout.Space();

			GUILayout.Label("Characters", EditorStyles.boldLabel);
			_scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.Height(100));
			_data.Characters = EditorGUILayout.TextArea(_data.Characters, GUILayout.ExpandHeight(true));
			GUILayout.EndScrollView();

			EditorGUILayout.Space();
			_data.DefaultCharacterSpacing = EditorGUILayout.IntField("Character Spacing", _data.DefaultCharacterSpacing);

			EditorGUILayout.Space();
			GUILayout.Label("Custom Character Properties", EditorStyles.boldLabel);
			_customCharPropsList.DoLayoutList();

			DrawCreateFontButton();

			GUILayout.EndVertical();
		}

		private void DrawCreateFontButton()
		{
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUI.color = Color.cyan;

			if (GUILayout.Button("Create Font", _buttonStyle))
				BitmapFontCreator.CreateFont(_data);

			GUI.color = Color.white;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
	}
}
