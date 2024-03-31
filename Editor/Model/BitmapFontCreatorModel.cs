using System;
using System.Collections.Generic;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
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

	internal class BitmapFontCreatorData
	{
		public string Characters;
		public Orientation Orientation;
		public int Cols;
		public int Rows;
		public float AlphaThreshold;
		public bool Monospaced;
		public int DefaultCharacterSpacing;
		public List<CharacterProps> CustomCharacterProps;

		public virtual void CopyTo(BitmapFontCreatorData dest)
		{
			if (dest == null) return;

			dest.Characters = Characters;
			dest.Orientation = Orientation;
			dest.Cols = Cols;
			dest.Rows = Rows;
			dest.AlphaThreshold = AlphaThreshold;
			dest.Monospaced = Monospaced;
			dest.DefaultCharacterSpacing = DefaultCharacterSpacing;

			if (dest.CustomCharacterProps != null) dest.CustomCharacterProps.Clear();
			else dest.CustomCharacterProps = new List<CharacterProps>();

			foreach (var e in CustomCharacterProps)
				dest.CustomCharacterProps.Add(new CharacterProps() { Character = e.Character, Spacing = e.Spacing });
		}
	}

	internal class ExecutionData : BitmapFontCreatorData
	{
		public Texture2D Texture;

		public static ExecutionData Default => new()
		{
			Texture = null,
			Characters = "",
			Orientation = Orientation.Horizontal,
			Cols = 1,
			Rows = 1,
			AlphaThreshold = 0f,
			DefaultCharacterSpacing = 10,
			Monospaced = false,
			CustomCharacterProps = new List<CharacterProps>(),
		};
	}
}
