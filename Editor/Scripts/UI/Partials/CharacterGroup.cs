using UnityEditor;
using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	public partial class BitmapFontCreatorEditor
	{
		private int _selectedCharacterSetIndex = 0;

		private void DrawCharacterGroup()
		{
			EditorGUILayout.BeginVertical(Styles.Group);

			_data.CaseInsentive = EditorGUILayout.Toggle(UIContent.CaseInsentive, _data.CaseInsentive);
			_data.DefaultCharacterSpacing = EditorGUILayout.IntField(UIContent.DefaultCharacterSpacing, _data.DefaultCharacterSpacing);

			DrawCharacterSetDropDown();

			EditorGUILayout.Space();
			DrawCharactersField();

			EditorGUILayout.Space();
			GUILayout.Label(UIContent.CustomCharacterProperties, Styles.HeaderLabel);
			_customCharPropsList.DoLayoutList();

			EditorGUILayout.EndVertical();
		}

		private void DrawCharacterSetDropDown()
		{
			_selectedCharacterSetIndex = EditorGUILayout.Popup(UIContent.CharacterSet, _selectedCharacterSetIndex, CharacterSets.Names);
			if (!GUI.changed) return;
			if (_selectedCharacterSetIndex == 0) return;
			_data.Characters = CharacterSets.Characters[_selectedCharacterSetIndex];
		}

		private void DrawCharactersField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(UIContent.Characters, Styles.HeaderLabel);
			GUILayout.FlexibleSpace();
			GUILayout.Label(_data.ValidCharactersCount.ToString(), _data.ValidCharactersCount == _data.Cols * _data.Rows ? Styles.CounterLabelRight : Styles.CounterLabelWrong);
			GUILayout.EndHorizontal();

			_charactersScrollPos = GUILayout.BeginScrollView(_charactersScrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Height(100));

			EditorGUI.BeginChangeCheck();
			_data.Characters = GUILayout.TextArea(_data.Characters, Styles.CharactersField);
			if (EditorGUI.EndChangeCheck())
				_selectedCharacterSetIndex = 0;

			GUILayout.EndScrollView();
		}
	}
}