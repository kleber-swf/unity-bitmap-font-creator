using System;
using System.Collections.Generic;
using UnityEngine;

namespace kleberswf.tools.bitmapfontcreator
{
	internal enum Orientation
	{
		Horizontal,
		Vertical,
	}

	[Serializable]
	internal class CharacterProps
	{
		public string Character = "";
		public int Spacing = 0;
	}

	internal struct BitmapFontCreatorData
	{
		public Texture2D Texture;
		public string Characters;
		public Orientation Orientation;
		public int Cols;
		public int Rows;
		public float AlphaThreshold;
		// public int LineSpacing;
		public bool Monospaced;
		public int DefaultCharacterSpacing;
		public List<CharacterProps> CustomCharacterProps;

		public static BitmapFontCreatorData Default => new()
		{
			Texture = null,
			Characters = "$ 0123456789 . ",
			Orientation = Orientation.Horizontal,
			Cols = 3,
			Rows = 5,
			AlphaThreshold = 0f,
			DefaultCharacterSpacing = 10,
			Monospaced = false,
			CustomCharacterProps = new List<CharacterProps>() { new() { Character = ".", Spacing = 0 } },
		};
	}
}
