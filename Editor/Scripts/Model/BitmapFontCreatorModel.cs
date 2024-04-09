using System;
using System.Collections.Generic;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	[Serializable]
	internal class BitmapFontCreatorModel : ISerializationCallbackReceiver
	{
		private const string IgnoreCharacter = "\n";

		[SerializeField] private string _characters = string.Empty;
		public Orientation Orientation = Orientation.Horizontal;
		public int Cols = 1;
		public int Rows = 1;
		public float AlphaThreshold = 0f;
		public bool Monospaced = false;
		public bool CaseInsentive = false;
		public int Ascent = 0;
		public int Descent = 0;
		public int FontSize = 0;
		public bool AutoFontSize = true;
		public int LineSpacing = 0;
		public bool AutoLineSpacing = true;
		public int DefaultCharacterSpacing = 0;
		public List<CharacterProps> CustomCharacterProps = new();

		public BitmapFontCreatorModel() { }

		public BitmapFontCreatorModel(BitmapFontCreatorModel from) { from?.CopyTo(this); }

		public string Characters
		{
			get { return _characters; }
			set
			{
				if (_characters == value) return;
				_characters = value;
				UpdateChacters();
			}
		}

		public string ValidCharacters { get; protected set; } = string.Empty;
		public int ValidCharactersCount { get; protected set; } = 0;

		public virtual void CopyTo(BitmapFontCreatorModel dest)
		{
			if (dest == null) return;

			dest.Characters = _characters;
			dest.Orientation = Orientation;
			dest.Cols = Cols;
			dest.Rows = Rows;
			dest.AlphaThreshold = AlphaThreshold;
			dest.LineSpacing = LineSpacing;
			dest.AutoLineSpacing = AutoLineSpacing;
			dest.FontSize = FontSize;
			dest.AutoFontSize = AutoFontSize;
			dest.Monospaced = Monospaced;
			dest.CaseInsentive = CaseInsentive;
			dest.Ascent = Ascent;
			dest.Descent = Descent;
			dest.DefaultCharacterSpacing = DefaultCharacterSpacing;

			if (dest.CustomCharacterProps != null) dest.CustomCharacterProps.Clear();
			else dest.CustomCharacterProps = new List<CharacterProps>();

			foreach (var e in CustomCharacterProps)
				dest.CustomCharacterProps.Add(e.Clone());

			UpdateChacters();
		}

		private void UpdateChacters()
		{
			ValidCharacters = _characters.Replace(IgnoreCharacter, string.Empty);
			ValidCharactersCount = ValidCharacters.Length;
		}

		public void OnBeforeSerialize() { }
		public void OnAfterDeserialize() { UpdateChacters(); }
	}
}
