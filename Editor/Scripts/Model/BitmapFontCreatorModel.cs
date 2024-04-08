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
		public Orientation Orientation;
		public int Cols;
		public int Rows;
		public float AlphaThreshold;
		public bool Monospaced;
		public bool CaseInsentive;
		public float Ascent;
		public float Descent;
		public float FontSize;
		public bool AutoFontSize;
		public float LineSpacing;
		public bool AutoLineSpacing;
		public int DefaultCharacterSpacing;
		public List<CharacterProps> CustomCharacterProps;

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

		// TODO this should be called automatically when the field Characters changes
		private void UpdateChacters()
		{
			ValidCharacters = _characters.Replace(IgnoreCharacter, string.Empty);
			ValidCharactersCount = ValidCharacters.Length;
		}

		public void OnBeforeSerialize() { }
		public void OnAfterDeserialize() { UpdateChacters(); }

		public static BitmapFontCreatorModel Default => new()
		{
			Characters = string.Empty,
			Orientation = Orientation.Horizontal,
			Cols = 1,
			Rows = 1,
			AlphaThreshold = 0f,
			LineSpacing = 0f,
			AutoLineSpacing = true,
			FontSize = 0f,
			AutoFontSize = true,
			Monospaced = false,
			CaseInsentive = false,
			Ascent = 0f,
			Descent = 0f,
			DefaultCharacterSpacing = 0,
			CustomCharacterProps = new List<CharacterProps>(),
			ValidCharacters = string.Empty,
			ValidCharactersCount = 0
		};
	}
}
