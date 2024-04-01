using UnityEngine;
using UnityEditor;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	public class BitmapFontCreatorEditor : EditorWindow
	{
		public const string MenuItemPath = "Window/Bitmap Font Creator";

		private readonly ExecutionData _data = ExecutionData.Default;
		private CharacterPropsList _customCharPropsList;
		private ProfilesView _profilesView;
		private PrefsView _prefsView;
		private Settings _settings;

		private Vector2 _charactersScrollPos = Vector2.zero;
		private Vector2 _mainScrollPos = Vector2.zero;
		private int _selectedCharacterSetIndex = 0;

		[MenuItem(MenuItemPath)]
		public static void ShowWindow()
		{
			var size = new Vector2(300, 550);
			var window = GetWindowWithRect<BitmapFontCreatorEditor>(
				new Rect((Screen.width - size.x) * 0.5f, (Screen.height - size.y) * 0.5f, size.x, size.y),
				false,
				"Bitmap Font Creator",
				true
			);
			window.Setup();
			window.minSize = Vector2.zero;
			window.maxSize = new Vector2(1000, 1000);
		}

		private void Setup()
		{
			_settings = Settings.Load();
			var prefs = PrefsModel.Load();
			_settings.Profiles.Selected?.CopyTo(_data);

			_customCharPropsList = new CharacterPropsList(_data.CustomCharacterProps);
			_profilesView = new ProfilesView(_data, _settings.Profiles, prefs);
			_prefsView = new PrefsView(prefs);
		}

		private void OnGUI()
		{
#if BITMAP_FONT_CREATOR_DEBUG
			// Dev only
			if (_customCharPropsList == null) Setup();
#endif
			_mainScrollPos = GUILayout.BeginScrollView(_mainScrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandHeight(true));
			GUILayout.BeginVertical();

			_data.Texture = EditorGUILayout.ObjectField(UI.Texture, _data.Texture, typeof(Texture2D), false) as Texture2D;
			_data.Orientation = (Orientation)EditorGUILayout.EnumPopup(UI.Orientation, _data.Orientation);

			_data.Cols = EditorGUILayout.IntField(UI.Cols, _data.Cols);
			_data.Rows = EditorGUILayout.IntField(UI.Rows, _data.Rows);
			_data.AlphaThreshold = EditorGUILayout.Slider(UI.AlphaThreshold, _data.AlphaThreshold, 0f, 1f);
			_data.Monospaced = EditorGUILayout.Toggle(UI.Monospaced, _data.Monospaced);

			EditorGUILayout.Space();
			DrawCharacterSetDropDown();

			EditorGUILayout.Space();
			DrawCharactersField();

			EditorGUILayout.Space();
			_data.DefaultCharacterSpacing = EditorGUILayout.IntField(UI.DefaultCharacterSpacing, _data.DefaultCharacterSpacing);

			EditorGUILayout.Space();
			GUILayout.Label(UI.CustomCharacterProperties, Styles.HeaderLabel);
			_customCharPropsList.DoLayoutList();

			DrawCreateFontButton();

			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.FlexibleSpace();

			DrawBottomMenu();
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
			var count = _data.Characters.Length;
			GUILayout.BeginHorizontal();
			GUILayout.Label(UI.Characters, Styles.HeaderLabel);
			GUILayout.FlexibleSpace();
			GUILayout.Label(count.ToString(), count == _data.Cols * _data.Rows ? Styles.CounterLabelRight : Styles.CounterLabelWrong);
			GUILayout.EndHorizontal();

			_charactersScrollPos = GUILayout.BeginScrollView(_charactersScrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Height(100));

			EditorGUI.BeginChangeCheck();
			_data.Characters = GUILayout.TextArea(_data.Characters, EditorStyles.textArea, GUILayout.ExpandHeight(true));
			if (EditorGUI.EndChangeCheck())
				_selectedCharacterSetIndex = 0;

			GUILayout.EndScrollView();

		}

		private void DrawCreateFontButton()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUI.color = Color.cyan;

			if (GUILayout.Button(UI.CreateButton, Styles.CreateButton))
				BitmapFontCreator.TryCreateFont(_data, _prefsView.Model.WarnOnReplaceFont);

			GUI.color = Color.white;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawBottomMenu()
		{
			GUILayout.BeginHorizontal(Styles.BottomMenu);
			_profilesView.Draw();
			if (GUILayout.Button(UI.RollbackButton, Styles.RollbackButton))
				RollbackSettings();
			GUILayout.FlexibleSpace();
			_prefsView.Draw();
			GUILayout.EndHorizontal();
		}

		private void RollbackSettings()
		{
			var profile = (BitmapFontCreatorData)_settings.Profiles.Selected ?? ExecutionData.Default;
			profile.CopyTo(_data);
		}
	}
}
