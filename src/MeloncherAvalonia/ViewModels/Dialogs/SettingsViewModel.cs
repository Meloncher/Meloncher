using System.Reactive;
using MeloncherCore.Settings;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels.Dialogs
{
	public class SettingsViewModel : ViewModelBase
	{
		public SettingsViewModel(LauncherSettings launcherSettings)
		{
			LauncherSettings = launcherSettings;
			OkCommand = ReactiveCommand.Create(OnOkCommandExecuted);
			ImportCommand = ReactiveCommand.Create(OnImportCommandExecuted);
		}

		[Reactive] public LauncherSettings LauncherSettings { get; set; }

		public ReactiveCommand<Unit, SettingsAction> ImportCommand { get; }

		public ReactiveCommand<Unit, SettingsAction> OkCommand { get; }

		private SettingsAction OnImportCommandExecuted()
		{
			return SettingsAction.Import;
		}

		private SettingsAction OnOkCommandExecuted()
		{
			return SettingsAction.None;
		}
	}

	public enum SettingsAction
	{
		None,
		Import
	}
}