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
		private PrefsModel _prefs;
		private string _error;

		private Vector2 _charactersScrollPos = Vector2.zero;
		private Vector2 _mainScrollPos = Vector2.zero;
		private int _selectedCharacterSetIndex = 0;

		private Vector2Int _guessRowsColsCache;

		[MenuItem(MenuItemPath)]
		public static void ShowWindow()
		{
			var size = new Vector2(310, 620);
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
			_prefs = PrefsModel.Load();
			_settings.Profiles.Selected?.CopyTo(_data);

			_customCharPropsList = new CharacterPropsList(_data.CustomCharacterProps);
			_profilesView = new ProfilesView(_data, _settings.Profiles, _prefs);
			_prefsView = new PrefsView(_prefs);
		}

		private void OnGUI()
		{
#if BITMAP_FONT_CREATOR_DEBUG
			if (_customCharPropsList == null) Setup();
#endif
			EditorGUIUtility.labelWidth = 120;
			_mainScrollPos = GUILayout.BeginScrollView(_mainScrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandHeight(true));
			GUILayout.BeginVertical();

			DrawTextureField();

			_data.Cols = UIUtils.IntFieldMin(UI.Cols, _data.Cols, 1);
			_data.Rows = UIUtils.IntFieldMin(UI.Rows, _data.Rows, 1);

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(UI.GuessRowsAndColsButton, Styles.MiniButton)) GuessRowsAndCols();
			var showPreview = DrawPreviewButton();
			GUILayout.EndHorizontal();

			EditorGUILayout.Space();
			_data.Orientation = (Orientation)EditorGUILayout.EnumPopup(UI.Orientation, _data.Orientation);
			_data.AlphaThreshold = EditorGUILayout.Slider(UI.AlphaThreshold, _data.AlphaThreshold, 0f, 1f);

			EditorGUILayout.Space();
			_data.Monospaced = EditorGUILayout.Toggle(UI.Monospaced, _data.Monospaced);
			_data.Ascent = UIUtils.FloatFieldMin(UI.Ascent, _data.Ascent, 0f);
			_data.Descent = UIUtils.FloatFieldMin(UI.Descent, _data.Descent, 0f);

			GUILayout.BeginHorizontal();
			_data.LineSpacing = EditorGUILayout.FloatField(UI.LineSpacing, _data.LineSpacing);
			if (GUILayout.Button(UI.GuessLineSpacingButton, Styles.MiniButton)) GuessLineSpacing();
			GUILayout.EndHorizontal();

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

			if (!string.IsNullOrEmpty(_error)) ShowCurrentError();
			if (showPreview) TexturePreviewPopup.Open(_data, _prefs);
		}

		private void ShowCurrentError()
		{
			Debug.LogError(_error);
			_error = null;
		}

		private void DrawTextureField()
		{
			EditorGUI.BeginChangeCheck();
			_data.Texture = EditorGUILayout.ObjectField(UI.Texture, _data.Texture, typeof(Texture2D), false) as Texture2D;
			if (EditorGUI.EndChangeCheck()) _guessRowsColsCache = Vector2Int.zero;
		}

		private bool DrawPreviewButton()
		{
			GUI.enabled = _data.Texture != null && _data.Rows > 0 && _data.Cols > 0;
			var value = GUILayout.Button(UI.PreviewButton, Styles.MiniButton);
			GUI.enabled = true;
			return value;
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
			GUILayout.BeginHorizontal();
			GUILayout.Label(UI.Characters, Styles.HeaderLabel);
			GUILayout.FlexibleSpace();
			GUILayout.Label(_data.ValidCharactersCount.ToString(), _data.ValidCharactersCount == _data.Cols * _data.Rows ? Styles.CounterLabelRight : Styles.CounterLabelWrong);
			GUILayout.EndHorizontal();

			_charactersScrollPos = GUILayout.BeginScrollView(_charactersScrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Height(100));

			EditorGUI.BeginChangeCheck();
			_data.Characters = GUILayout.TextArea(_data.Characters, Styles.CharactersField);
			if (EditorGUI.EndChangeCheck())
				_selectedCharacterSetIndex = 0;

			GUILayout.EndScrollView();
		}

		private void DrawCreateFontButton()
		{
			GUI.color = Color.cyan;
			if (UIUtils.CenteredButton(UI.CreateButton, Styles.CreateButton))
				BitmapFontCreator.TryCreateFont(_data, _prefsView.Model.WarnOnReplaceFont, out _error);
			GUI.color = Color.white;
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

		private void GuessRowsAndCols()
		{
			if (_data.Texture == null)
			{
				Debug.LogWarning("Texture cannot be null");
				return;
			}

			_guessRowsColsCache = _guessRowsColsCache.x == 0 || _guessRowsColsCache.y == 0
				? BitmapFontCreator.GuessRowsAndCols(_data.Texture)
				: _guessRowsColsCache;

			_data.Cols = _guessRowsColsCache.y;
			_data.Rows = _guessRowsColsCache.x;
		}

		private void GuessLineSpacing()
		{
			if (_data.Texture == null)
			{
				Debug.LogWarning("Texture cannot be null");
				return;
			}
			if (_data.Cols <= 0 || _data.Rows <= 0)
			{
				Debug.LogWarning("Rows and Cols must be greater than 0");
				return;
			}

			_data.LineSpacing = BitmapFontCreator.GuessLineSpacing(_data.Texture, _data.Rows);
		}
	}
}
