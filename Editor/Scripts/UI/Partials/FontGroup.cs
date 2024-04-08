using UnityEditor;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	public partial class BitmapFontCreatorEditor
	{
		private void DrawFontInfoGroup()
		{
			EditorGUILayout.BeginVertical(Styles.Group);

			_data.FontSize = UIUtils.FloatFieldWithAuto(UIContent.FontSize, _data.FontSize, UIContent.AutoFontSize, ref _data.AutoFontSize);
			_data.LineSpacing = UIUtils.FloatFieldWithAuto(UIContent.LineSpacing, _data.LineSpacing, UIContent.AutoLineSpacing, ref _data.AutoLineSpacing);
			_data.Ascent = UIUtils.FloatFieldMin(UIContent.Ascent, _data.Ascent, 0f);
			_data.Descent = UIUtils.FloatFieldMin(UIContent.Descent, _data.Descent, 0f);
			_data.Monospaced = EditorGUILayout.Toggle(UIContent.Monospaced, _data.Monospaced);

			EditorGUILayout.EndVertical();
		}
	}
}
