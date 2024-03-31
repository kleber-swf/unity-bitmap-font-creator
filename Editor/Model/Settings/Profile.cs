using System;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	[Serializable]
	internal class Profile : BitmapFontCreatorData
	{
		public string Name;
		public Profile() { }

		public Profile(string name, BitmapFontCreatorData src)
		{
			Name = name;
			src?.CopyTo(this);
		}
	}
}