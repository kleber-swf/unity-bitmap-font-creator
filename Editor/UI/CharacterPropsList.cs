using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal class CharacterPropsList : ReorderableList
	{
		private const float padding = 5f;

		public CharacterPropsList(List<CharacterProps> elements) : base(elements, typeof(CharacterProps), true, true, true, true)
		{
			drawHeaderCallback = ListDrawHeader;
			drawElementCallback = ListDrawElement;
			elementHeight = EditorGUIUtility.singleLineHeight;
		}

		private void ListDrawHeader(Rect rect)
		{
			rect.x += 14f;
			rect.width = rect.width * 0.5f - rect.x;
			GUI.Label(rect, "Character");

			rect.x += rect.width + padding * 2;
			GUI.Label(rect, "Spacing");
		}

		private void ListDrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			var item = list[index] as CharacterProps;

			rect.width = rect.width * 0.5f - padding;
			var text = EditorGUI.TextField(rect, item.Character, EditorStyles.label);
			if (GUI.changed) item.Character = text.Length > 1 ? text[1..] : text;

			rect.x += rect.width + padding;
			item.Spacing = EditorGUI.IntField(rect, item.Spacing, EditorStyles.label);
		}
	}
}
