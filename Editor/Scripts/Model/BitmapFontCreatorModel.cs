using System;
using System.Collections.Generic;
using UnityEditor;
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

	[Serializable]
	internal class BitmapFontCreatorData : ISerializationCallbackReceiver
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

		public virtual void CopyTo(BitmapFontCreatorData dest)
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

		public static BitmapFontCreatorData Default => new()
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

	[Serializable]
	internal class ExecutionData : BitmapFontCreatorData
	{
		public Texture2D Texture;

		public static new ExecutionData Default
		{
			get
			{
				var data = new ExecutionData { Texture = null };
				BitmapFontCreatorData.Default.CopyTo(data);
				return data;
			}
		}
	}
}
