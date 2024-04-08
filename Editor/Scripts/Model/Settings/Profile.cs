using System;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	[Serializable]
	internal class Profile : BitmapFontCreatorModel
	{
		public string Name;
		public Profile() { }

		public Profile(string name, BitmapFontCreatorModel src)
		{
			Name = name;
			src?.CopyTo(this);
		}
	}
}