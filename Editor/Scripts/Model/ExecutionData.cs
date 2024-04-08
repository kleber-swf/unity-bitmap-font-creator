using System;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	[Serializable]
	internal class ExecutionData : BitmapFontCreatorModel
	{
		public Texture2D Texture;

		public static new ExecutionData Default
		{
			get
			{
				var data = new ExecutionData { Texture = null };
				BitmapFontCreatorModel.Default.CopyTo(data);
				return data;
			}
		}
	}
}
