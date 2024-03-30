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

	internal class BitmapFontCreatorData
	{
		public string Characters;
		public Orientation Orientation;
		public int Cols;
		public int Rows;
		public float AlphaThreshold;
		// public int LineSpacing;
		public bool Monospaced;
		public int DefaultCharacterSpacing;
		public List<CharacterProps> CustomCharacterProps;

		public virtual void CopyFrom(BitmapFontCreatorData data)
		{
			Characters = data.Characters;
			Orientation = data.Orientation;
			Cols = data.Cols;
			Rows = data.Rows;
			AlphaThreshold = data.AlphaThreshold;
			Monospaced = data.Monospaced;
			DefaultCharacterSpacing = data.DefaultCharacterSpacing;

			if (CustomCharacterProps != null) CustomCharacterProps.Clear();
			else CustomCharacterProps = new List<CharacterProps>();

			foreach (var e in data.CustomCharacterProps)
				CustomCharacterProps.Add(new CharacterProps() { Character = e.Character, Spacing = e.Spacing });
		}
	}

	internal class ExecutionData : BitmapFontCreatorData
	{
		public Texture2D Texture;

		// TODO move this to ui
		public static ExecutionData Default => new()
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
