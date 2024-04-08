using System;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	[Serializable]
	internal class CharacterProps
	{
		public string Character = "";
		public Vector2Int Padding = Vector2Int.zero;
		public int Spacing = 0;

		public CharacterProps Clone()
		{
			return new CharacterProps
			{
				Character = Character,
				Padding = Padding,
				Spacing = Spacing,
			};
		}
	}
}
