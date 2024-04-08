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
}
