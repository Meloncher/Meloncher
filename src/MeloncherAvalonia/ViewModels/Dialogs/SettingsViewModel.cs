using MeloncherCore.Settings;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels.Dialogs
{
	public class SettingsViewModel : ViewModelBase
	{
		public SettingsViewModel(LauncherSettings launcherSettings)
		{
			LauncherSettings = launcherSettings;
		}

		[Reactive] public LauncherSettings LauncherSettings { get; set; }

		private void ImportCommand()
		{
			DialogHost.DialogHost.GetDialogSession("MainDialogHost")?.Close(SettingsAction.Import);
		}

		private void OkCommand()
		{
			DialogHost.DialogHost.GetDialogSession("MainDialogHost")?.Close(SettingsAction.None);
		}
	}

	public enum SettingsAction
	{
		None,
		Import
	}
}