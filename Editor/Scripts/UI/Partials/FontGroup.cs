using UnityEditor;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	public partial class BitmapFontCreatorEditor
	{
		private void DrawFontInfoGroup()
		{
			EditorGUILayout.BeginVertical(Styles.Group);

			_data.FontSize = UIUtils.IntFieldWithAuto(UIContent.FontSize, _data.FontSize, UIContent.AutoFontSize, ref _data.AutoFontSize);
			_data.LineSpacing = UIUtils.IntFieldWithAuto(UIContent.LineSpacing, _data.LineSpacing, UIContent.AutoLineSpacing, ref _data.AutoLineSpacing);
			_data.Ascent = UIUtils.IntFieldMin(UIContent.Ascent, _data.Ascent, 0);
			_data.Descent = UIUtils.IntFieldMin(UIContent.Descent, _data.Descent, 0);
			_data.Monospaced = EditorGUILayout.Toggle(UIContent.Monospaced, _data.Monospaced);

			EditorGUILayout.EndVertical();
		}
	}
}
