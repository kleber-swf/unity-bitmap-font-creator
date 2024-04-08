using UnityEngine;
using UnityEditor;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	public partial class BitmapFontCreatorEditor : EditorWindow
	{
		public const string MenuItemPath = "Window/Text/Bitmap Font Creator";

		private ExecutionData _data;
		private CharacterPropsList _customCharPropsList;
		private ProfilesView _profilesView;
		private PrefsView _prefsView;
		private Settings _settings;
		private PrefsModel _prefs;

		private Vector2 _charactersScrollPos = Vector2.zero;
		private Vector2 _mainScrollPos = Vector2.zero;
		private string _error = string.Empty;
		private bool _showPreview = false;
		private bool _textureGroupFoldout = true;
		private bool _fontGroupFoldout = true;
		private bool _charGroupFoldout = true;
		private Vector2Int _guessRowsColsCache;

		[MenuItem(MenuItemPath)]
		public static void ShowWindow()
		{
			var size = new Vector2(310, 800);
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
			_data = RuntimeData.Load().Data;
			_settings = Settings.Load();
			_prefs = PrefsModel.Load();

			if (!Application.isPlaying)
				_settings.Profiles.Selected?.CopyTo(_data);

			_customCharPropsList = new CharacterPropsList(_data.CustomCharacterProps);
			_profilesView = new ProfilesView(_data, _settings.Profiles, _prefs);
			_prefsView = new PrefsView(_prefs);
		}

		private void OnGUI()
		{
#if BITMAP_FONT_CREATOR_DEV
			if (_customCharPropsList == null) Setup();
#endif

			EditorGUIUtility.labelWidth = 120;
			_mainScrollPos = GUILayout.BeginScrollView(_mainScrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandHeight(true));
			GUILayout.BeginVertical();

			_textureGroupFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_textureGroupFoldout, UIContent.TextureGroupTitle, Styles.GroupTitle);
			if (_textureGroupFoldout) DrawTextureGroup();
			EditorGUILayout.EndFoldoutHeaderGroup();

			_fontGroupFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_fontGroupFoldout, UIContent.FontGroupTitle, Styles.GroupTitle);
			if (_fontGroupFoldout) DrawFontInfoGroup();
			EditorGUILayout.EndFoldoutHeaderGroup();

			_charGroupFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_charGroupFoldout, UIContent.CharactersGroupTitle, Styles.GroupTitle);
			if (_charGroupFoldout) DrawCharacterGroup();
			EditorGUILayout.EndFoldoutHeaderGroup();

			DrawCreateFontButton();

			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.FlexibleSpace();

			DrawBottomMenu();

			if (!string.IsNullOrEmpty(_error)) ShowCurrentError();
			if (_showPreview) TexturePreviewPopup.Open(_data, _prefs);
		}

		private void DrawCreateFontButton()
		{
			GUI.color = Color.cyan;
			if (UIUtils.CenteredButton(UIContent.CreateButton, Styles.CreateButton))
				BitmapFontCreator.TryCreateFont(_data, _prefsView.Model.WarnOnReplaceFont, out _error);
			GUI.color = Color.white;
		}

		private void ShowCurrentError()
		{
			Debug.LogError(_error);
			_error = null;
		}
	}
}
