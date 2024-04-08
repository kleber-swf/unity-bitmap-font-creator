using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal static class Styles
	{
		public static readonly float AutoToggleWidth = 50f;

		public static readonly Font MonospacedFont = Resources.Load<Font>("Fonts/monospaced");
		public static readonly Texture2D GridDarkTexture = Resources.Load<Texture2D>("Textures/grid-dark");
		public static readonly Texture2D GridLightTexture = Resources.Load<Texture2D>("Textures/grid-light");

		public static readonly GUIStyle GroupTitle = new(EditorStyles.foldoutHeader)
		{
			margin = new(0, 0, 2, 2),
			padding = new(18, 5, 10, 10),
		};

		public static readonly GUIStyle Group = new("hostview")
		{
			margin = new(),
			padding = new(18, 5, 5, 20),
		};

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
			font = MonospacedFont,
			stretchHeight = true,
			normal = new() { textColor = Color.white },
		};

		public static readonly GUIStyle CustomCharacterField = new(EditorStyles.textField)
		{
			font = MonospacedFont,
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
		public static readonly GUIStyle ToolbarButton = new("TE toolbarbutton") { padding = new(10, 6, 0, 0), };
		public static readonly GUIStyle PreferencesButton = new("PaneOptions") { margin = new(0, 0, 3, 0), };

		public static readonly GUIStyle CenterLabel = new(GUI.skin.label)
		{
			stretchWidth = true,
			stretchHeight = true,
			alignment = TextAnchor.MiddleCenter,
		};

		public static readonly GUIStyle Toolbar = new("TimeAreaToolbar") { fixedHeight = 24, };
		public static readonly GUIStyle MiniButton = new(EditorStyles.miniButton) { fixedWidth = 66, };

		public static readonly GUIStyle ToolbarLabel = new(EditorStyles.label)
		{
			margin = new(0, 20, 0, 0),
			padding = new(0, 3, 0, 0),
			alignment = TextAnchor.MiddleLeft,
			stretchHeight = true,
		};

		public static readonly GUIStyle BackgroundTextureIcon = new("HelpBox")
		{
			padding = new(2, 2, 2, 2),
			imagePosition = ImagePosition.ImageOnly,
			fixedWidth = 20,
			fixedHeight = 20,
		};
	}
}
