using UnityEditor;
using UnityEngine;

namespace kleberswf.tools.bitmapfontcreator
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
		public static readonly GUIContent CreateFont = new("Create", "Create the font");
		public static readonly GUIContent Profile = new("Profiles", "List of saved profiles");
	}

	internal static class Styles
	{
		public static readonly GUIStyle Header = new(EditorStyles.label)
		{
			margin = new(EditorStyles.label.margin.left, EditorStyles.label.margin.right, 5, 5),
		};

		public static readonly GUIStyle CreateButton = new(EditorStyles.miniButton)
		{
			fontSize = 12,
			padding = new(20, 20, 8, 8),
			margin = new(10, 10, 20, 20),
			fixedHeight = 32,
		};
	}
}