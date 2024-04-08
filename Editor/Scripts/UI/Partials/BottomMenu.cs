using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	public partial class BitmapFontCreatorEditor
	{

		private void SetupBottomMenu(ExecutionData data, ProfileList profiles, PrefsModel prefs)
		{
			_customCharPropsList = new CharacterPropsList(data.CustomCharacterProps);
			_profilesView = new ProfilesView(data, profiles, prefs);
			_prefsView = new PrefsView(prefs);
		}

		private void DrawBottomMenu()
		{
			GUILayout.BeginHorizontal(Styles.BottomMenu);
			_profilesView.Draw();

			if (GUILayout.Button(UIContent.RollbackButton, Styles.ToolbarButton))
				RollbackSettings();

			if (GUILayout.Button(UIContent.DefaultButton, Styles.ToolbarButton))
				ClearSettings();

			GUILayout.FlexibleSpace();

			_prefsView.Draw();
			GUILayout.EndHorizontal();
		}

		private void RollbackSettings()
		{
			var profile = (BitmapFontCreatorModel)_settings.Profiles.Selected ?? ExecutionData.Default;
			profile.CopyTo(_data);
		}

		private void ClearSettings()
		{
			ExecutionData.Default.CopyTo(_data);
		}
	}
}
