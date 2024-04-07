using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal static class CharacterSets
	{
		public static readonly string[] Names = new[] {
			"Custom",
			"ASCII",
			"ASCII Lowercase",
			"ASCII Uppercase",
			"Numbers",
		};
		public static readonly string[] Characters = new[] {
			null,
			"!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~",
			"!\"#$%&'()*+,-./0123456789:;<=>?@[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~",
			"!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`{|}~",
			"0123456789.,-+",
		};
	}

	internal static class UI
	{
		public static readonly Font MonospacedFont = Resources.Load<Font>("Fonts/monospaced");
		public static readonly Texture2D GridDarkTexture = Resources.Load<Texture2D>("Textures/grid-dark");
		public static readonly Texture2D GridLightTexture = Resources.Load<Texture2D>("Textures/grid-light");

		public static readonly GUIContent Texture = new("Font Texture", "Texture used for the font");
		public static readonly GUIContent Orientation = new("Orientation", "Order to look up for characters in the texture");
		public static readonly GUIContent Cols = new("Cols", "Number of columns in the texture");
		public static readonly GUIContent Rows = new("Rows", "Number of rows in the texture");
		public static readonly GUIContent GuessRowsAndColsButton = new("Guess", "Guess the number of rows and columns of the texture based on transparency gaps");
		public static readonly GUIContent PreviewButton = new("Preview", "Preview the texture with current rows and cols");
		public static readonly GUIContent AlphaThreshold = new("Alpha Threshold", "Alpha threshold to identify characters bounds");
		public static readonly GUIContent Monospaced = new("Monospaced", "Whether the result font should be monospaced");
		public static readonly GUIContent CaseInsentive = new("Case Insentive", "Whether the characters for lowercase and uppercase are the same");
		public static readonly GUIContent Ascent = new("Ascent", "Font ascent. It's the part of the glyphs that should be above the baseline");
		public static readonly GUIContent Descent = new("Descent", "Font descent. It's the part of the glyphs that should be below the baseline");
		public static readonly GUIContent FontSize = new("Font Size", "Base font size in pixels");
		public static readonly GUIContent AutoFontSize = new("Auto", "Automatically calculate the font size based on ascent property or cell size");
		public static readonly GUIContent LineSpacing = new("Line Spacing", "Vertical spacing between lines");
		public static readonly GUIContent AutoLineSpacing = new("Auto", "Automatically calculate the line spacing based on cell height");
		public static readonly GUIContent CharacterSet = new("Character Set", "Predefined character set to use");
		public static readonly GUIContent Characters = new("Characters", "Characters used in the font in order they appear in the texture. Use the space character to represent blank spaces in the texture");
		public static readonly GUIContent DefaultCharacterSpacing = new("Character Spacing", "Default spacing between characters");
		public static readonly GUIContent CustomCharacterProperties = new("Custom Character Properties", "Custom properties for each characters, if any");
		public static readonly GUIContent CustomCharacter = new("Char", "Character to apply the properties");
		public static readonly GUIContent CustomSpacing = new("Spacing", "Horizontal spacing after the character (advance). Ignored if font is monospaced");
		public static readonly GUIContent CustomPadding = new("Padding", "Custom horizontal and vertical padding to add before the character");

		public static readonly GUIContent CreateButton = new("Create", "Create font files");

		public static readonly GUIContent RollbackButton = new("Rollback", "Rollback settings to the selected profile or default");
		public static readonly GUIContent PreferencesButton = new(string.Empty, "Preferences");
		public static readonly GUIContent PreferencesLabel = new("Preferences");
		public static readonly GUIContent WarnOnReplaceFont = new("Warn before replacing font files", "Warn before replacing an existing font. This will replace the old font keeping the references");
		public static readonly GUIContent WarnOnReplaceSettings = new("Warn before replacing settings", "Warn before replacing settings when selecting a profile");
		public static readonly GUIContent WarnOnReplaceProfile = new("Warn before replacing profile", "Warn before replacing an existing profile");

		public static readonly GUIContent NoContent = new("Invalid texture", "There is no texture selected or the number of rows or columns are set to zero");
		public static readonly GUIContent GridColorLabel = new("Grid", "Color for the grid lines showing rows and columns");
		public static readonly GUIContent HeightColorLabel = new("Height", "Color for de glyph hight line (ascent + descent)");
		public static readonly GUIContent BaselineColorLabel = new("Baseline", "Color for the baseline (descent)");

		public static readonly GUIContent DarkTextureIcon = new() { image = GridDarkTexture, };
		public static readonly GUIContent LightTextureIcon = new() { image = GridLightTexture };
	}

	internal static class Styles
	{
		public static readonly float AutoToggleWidth = 50f;

		public static readonly GUIStyle HeaderLabel = new(EditorStyles.label)
		{
			margin = new(EditorStyles.label.margin.left, EditorStyles.label.margin.right, 5, 5),
		};

		public static readonly GUIStyle SectionLabel = new("ContentToolbar")
		{
			padding = new(5, 5, 5, 5),
			fixedHeight = 25,
		};

		public static readonly GUIStyle CounterLabelRight = new(EditorStyles.label)
		{
			stretchHeight = true,
			alignment = TextAnchor.LowerCenter,
			fontStyle = FontStyle.Italic,
			normal = new()
			{
				background = new GUIStyle("WinBtnMaxMac").normal.background,
				textColor = EditorStyles.label.normal.textColor,
			},
			imagePosition = ImagePosition.ImageLeft,
			contentOffset = new Vector2(-17, 0),
		};

		public static readonly GUIStyle CounterLabelWrong = new(EditorStyles.label)
		{
			stretchHeight = true,
			stretchWidth = true,
			alignment = TextAnchor.LowerCenter,
			fontStyle = FontStyle.Italic,
			normal = new()
			{
				background = new GUIStyle("WinBtnCloseMac").normal.background,
				textColor = EditorStyles.label.normal.textColor,
			},
			imagePosition = ImagePosition.ImageLeft,
			contentOffset = new Vector2(-17, 0),
		};

		public static readonly GUIStyle CharactersField = new(EditorStyles.textArea)
		{
			font = UI.MonospacedFont,
			stretchHeight = true,
			normal = new() { textColor = Color.white },
		};

		public static readonly GUIStyle CustomCharacterField = new(EditorStyles.textField)
		{
			font = UI.MonospacedFont,
		};

		public static readonly GUIStyle CreateButton = new(GUI.skin.button)
		{
			padding = new(20, 20, 7, 8),
			margin = new(10, 10, 20, 20),
			fixedHeight = 32,
		};

		public static readonly GUIStyle BottomMenu = new("DD Background")
		{
			padding = new(1, 1, 1, 1),
			margin = new(),
		};

		public static readonly GUIStyle ProfilesButton = new("ToolbarPopupLeft");
		public static readonly GUIStyle RollbackButton = new("TE toolbarbutton") { padding = new(10, 6, 0, 0), };
		public static readonly GUIStyle PreferencesButton = new("PaneOptions") { margin = new(0, 0, 3, 0), };

		public static readonly GUIStyle CenterLabel = new(GUI.skin.label)
		{
			stretchWidth = true,
			stretchHeight = true,
			alignment = TextAnchor.MiddleCenter,
		};

		public static readonly GUIStyle Toolbar = new("TimeAreaToolbar") { fixedHeight = 24, };
		public static readonly GUIStyle MiniButton = new(EditorStyles.miniButton) { fixedWidth = 66, };


		public static readonly GUIStyle BackgroundTextureIcon = new("HelpBox")
		{
			padding = new(2, 2, 2, 2),
			imagePosition = ImagePosition.ImageOnly,
			fixedWidth = 20,
			fixedHeight = 20,
		};
	}

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
