using UnityEngine;
using UnityEditor;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	// TODO save last configuration
	// TODO pre configured profiles
	// TODO save profiles to the project
	// TODO no error on play
	public class BitmapFontCreatorEditor : EditorWindow
	{
		private readonly ExecutionData _data = ExecutionData.Default;
		private BitmapFontCreatorPrefs _prefs;
		private CharacterPropsList _customCharPropsList;
		private ProfilesView _profilesView;

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
			_prefs = BitmapFontCreatorPrefs.Load();
			_customCharPropsList = new CharacterPropsList(_data.CustomCharacterProps);
			_profilesView = new ProfilesView(_data, _prefs.Profiles);
		}

		private void OnGUI()
		{
			// TODO use _initialized when finished
			if (_customCharPropsList == null) Initialize();

			_mainScrollPos = GUILayout.BeginScrollView(_mainScrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandHeight(true));
			GUILayout.BeginVertical();

			_data.Texture = EditorGUILayout.ObjectField(UI.Texture, _data.Texture, typeof(Texture2D), false) as Texture2D;
			_data.Orientation = (Orientation)EditorGUILayout.EnumPopup(UI.Orientation, _data.Orientation);

			_data.Cols = EditorGUILayout.IntField(UI.Cols, _data.Cols);
			_data.Rows = EditorGUILayout.IntField(UI.Rows, _data.Rows);
			_data.AlphaThreshold = EditorGUILayout.Slider(UI.AlphaThreshold, _data.AlphaThreshold, 0f, 1f);
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

			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.FlexibleSpace();

			DrawBottomMenu();
		}

		private void DrawBottomMenu()
		{
			GUILayout.BeginHorizontal(Styles.BottomMenu);
			_profilesView.Draw();
			GUILayout.EndHorizontal();
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
