using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	public partial class BitmapFontCreatorEditor
	{
		private void DrawBottomMenu()
		{
			GUILayout.BeginHorizontal(Styles.BottomMenu);
			_profilesView.Draw();
			if (GUILayout.Button(UIContent.RollbackButton, Styles.RollbackButton))
				RollbackSettings();
			GUILayout.FlexibleSpace();
			_prefsView.Draw();
			GUILayout.EndHorizontal();
		}

		private void RollbackSettings()
		{
			var profile = (BitmapFontCreatorModel)_settings.Profiles.Selected ?? ExecutionData.Default;
			profile.CopyTo(_data);
		}
	}
}
