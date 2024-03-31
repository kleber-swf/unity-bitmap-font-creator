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
		public static readonly GUIContent Texture = new("Font Texture", "Texture used for the font");
		public static readonly GUIContent Orientation = new("Orientation", "Order to look up for characters in the texture");
		public static readonly GUIContent Cols = new("Cols", "Number of columns in the texture");
		public static readonly GUIContent Rows = new("Rows", "Number of rows in the texture");
		public static readonly GUIContent AlphaThreshold = new("Alpha Threshold", "Alpha threshold to identify characters bounds");
		public static readonly GUIContent Monospaced = new("Monospaced", "Whether the result font should be monospaced");
		public static readonly GUIContent CharacterSet = new("Character Set", "Predefined character set to use");
		public static readonly GUIContent Characters = new("Characters", "Characters used in the font in order they appear in the texture. Use the space character to represent blank spaces in the texture");
		public static readonly GUIContent DefaultCharacterSpacing = new("Character Spacing", "Default spacing between characters");
		public static readonly GUIContent CustomCharacterProperties = new("Custom Character Properties", "Custom properties for each characters, if any");
		public static readonly GUIContent CreateButton = new("Create", "Create font files");

		public static readonly GUIContent RollbackButton = new("Rollback", "Rollback settings to the selected profile or default");
		public static readonly GUIContent PreferencesButton = new(string.Empty, "Preferences");
		public static readonly GUIContent PreferencesLabel = new("Preferences");
		public static readonly GUIContent WarnOnReplaceFont = new("Warn before replacing font files", "Warn before replacing an existing font. This will replace the old font keeping the references");
		public static readonly GUIContent WarnOnReplaceSettings = new("Warn before replacing settings", "Warn before replacing settings when selecting a profile");
		public static readonly GUIContent WarnOnReplaceProfile = new("Warn before replacing profile", "Warn before replacing an existing profile");
	}

	internal static class Styles
	{
		public static readonly GUIStyle HeaderLabel = new(EditorStyles.label)
		{
			margin = new(EditorStyles.label.margin.left, EditorStyles.label.margin.right, 5, 5),
		};

		public static readonly GUIStyle SectionLabel = new("ContentToolbar")
		{
			padding = new(5, 5, 5, 5),
			fixedHeight = 25,
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

		public static readonly GUIStyle RollbackButton = new("TE toolbarbutton")
		{
			padding = new(10, 6, 0, 0),
		};

		public static readonly GUIStyle PreferencesButton = new("PaneOptions")
		{
			margin = new(0, 0, 3, 0),
		};
	}
}
