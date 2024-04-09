using System;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	[Serializable]
	internal class ExecutionData : BitmapFontCreatorModel
	{
		public Texture2D Texture = null;

		public ExecutionData() : base() { }
		public ExecutionData(BitmapFontCreatorModel from) : base(from) { }

		public override void CopyTo(BitmapFontCreatorModel to)
		{
			base.CopyTo(to);
			if (to is ExecutionData t) t.Texture = Texture;
		}
	}
}
