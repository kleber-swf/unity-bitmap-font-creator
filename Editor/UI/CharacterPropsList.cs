using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal class CharacterPropsList : ReorderableList
	{
		private const float padding = 14f;
		private const float spacing = 5f;
		private static readonly float[] widths = new float[] { 0.2f, 0.2f, 0.6f };

		public CharacterPropsList(List<CharacterProps> elements) : base(elements, typeof(CharacterProps), true, true, true, true)
		{
			drawHeaderCallback = ListDrawHeader;
			drawElementCallback = ListDrawElement;
			elementHeight = EditorGUIUtility.singleLineHeight;
		}

		private void ListDrawHeader(Rect rect)
		{
			rect.x += padding;
			var w = rect.width - padding;

			rect.width = widths[0] * w;
			GUI.Label(rect, UI.CustomCharacter);

			rect.x += rect.width + spacing;
			rect.width = widths[1] * w;
			GUI.Label(rect, UI.CustomSpacing);

			rect.x += rect.width + spacing;
			rect.width = widths[2] * w;
			GUI.Label(rect, UI.CustomPadding);
		}

		private void ListDrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			var item = list[index] as CharacterProps;

			var w = rect.width;

			rect.width = widths[0] * w;
			var text = EditorGUI.TextField(rect, item.Character);
			if (GUI.changed) item.Character = text.Length > 1 ? text[1..] : text;

			rect.x += rect.width + spacing;
			rect.width = widths[1] * w - spacing;
			item.Spacing = EditorGUI.IntField(rect, item.Spacing);

			rect.x += rect.width + spacing * 2;
			rect.width = widths[2] * w - spacing * 2;
			item.Padding = EditorGUI.Vector2IntField(rect, string.Empty, item.Padding);
		}
	}
}
