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
		private readonly ExecutionData _data = ExecutionData.Default;
		private CharacterPropsList _customCharPropsList;
		private ProfilesUI _profiles;

		private bool _initialized = false;
		private Vector2 _charactersScrollPos = Vector2.zero;
		private Vector2 _mainScrollPos = Vector2.zero;
		private int _selectedCharacterSetIndex = 0;  // TODO put this inside data

		[MenuItem("Window/Bitmap Font Creator")]
		private static void ShowWindow()
		{
			var window = GetWindow<BitmapFontCreatorEditor>();
			window.titleContent = new GUIContent("Bitmap Font Creator");
			window.minSize = new Vector2(300, 300);
			window.Show();
		}

		private void Initialize()
		{
			_initialized = true;
			_customCharPropsList = new CharacterPropsList(_data.CustomCharacterProps);
			_profiles = new ProfilesUI(_data);
		}

		private void OnGUI()
		{
			// TODO use _initialized when finished
			if (_customCharPropsList == null) Initialize();

			GUILayout.BeginVertical();
			_mainScrollPos = GUILayout.BeginScrollView(_mainScrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandHeight(true));

			_data.Texture = EditorGUILayout.ObjectField(UI.Texture, _data.Texture, typeof(Texture2D), false) as Texture2D;
			_data.Orientation = (Orientation)EditorGUILayout.EnumPopup(UI.Orientation, _data.Orientation);

			_data.Cols = EditorGUILayout.IntField(UI.Cols, _data.Cols);
			_data.Rows = EditorGUILayout.IntField(UI.Rows, _data.Rows);
			_data.AlphaThreshold = EditorGUILayout.FloatField(UI.AlphaThreshold, _data.AlphaThreshold);
			_data.Monospaced = EditorGUILayout.Toggle(UI.Monospaced, _data.Monospaced);
			// _data.LineSpacing = EditorGUILayout.IntField("Line Spacing", _data.LineSpacing);

			EditorGUILayout.Space();

			DrawCharacterSetDropDown();

			GUILayout.Label(UI.Characters, Styles.Header);
			_charactersScrollPos = GUILayout.BeginScrollView(_charactersScrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Height(100));
			EditorGUI.BeginChangeCheck();
			DrawCharactersField();
			GUILayout.EndScrollView();

			EditorGUILayout.Space();
			_data.DefaultCharacterSpacing = EditorGUILayout.IntField(UI.DefaultCharacterSpacing, _data.DefaultCharacterSpacing);

			EditorGUILayout.Space();
			GUILayout.Label(UI.CustomCharacterProperties, Styles.Header);
			_customCharPropsList.DoLayoutList();

			DrawCreateFontButton();

			GUILayout.EndScrollView();
			GUILayout.FlexibleSpace();
			_profiles.Draw();
			GUILayout.EndVertical();
		}

		private void DrawCharacterSetDropDown()
		{
			_selectedCharacterSetIndex = EditorGUILayout.Popup(UI.CharacterSet, _selectedCharacterSetIndex, CharacterSets.Names);
			if (!GUI.changed) return;
			if (_selectedCharacterSetIndex == 0) return;
			_data.Characters = CharacterSets.Characters[_selectedCharacterSetIndex];
		}

		private void DrawCharactersField()
		{
			_data.Characters = GUILayout.TextArea(_data.Characters, EditorStyles.textArea, GUILayout.ExpandHeight(true));
			if (EditorGUI.EndChangeCheck())
				_selectedCharacterSetIndex = 0;
		}

		private void DrawCreateFontButton()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUI.color = Color.cyan;

			if (GUILayout.Button(UI.CreateFont, Styles.CreateButton))
				BitmapFontCreator.CreateFont(_data);

			GUI.color = Color.white;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
	}
}
