using System.Reactive;
using MeloncherCore.Settings;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		[Reactive] public LauncherSettings LauncherSettings { get; set; }

		public SettingsViewModel(LauncherSettings launcherSettings)
		{
			LauncherSettings = launcherSettings;
			OkCommand = ReactiveCommand.Create(OnOkCommandExecuted);
			ImportCommand = ReactiveCommand.Create(OnImportCommandExecuted);
		}

		public SettingsViewModel()
		{
		}

		public ReactiveCommand<Unit, SettingsAction> ImportCommand { get; }

		private SettingsAction OnImportCommandExecuted()
		{
			return SettingsAction.Import;
		}

		public ReactiveCommand<Unit, SettingsAction> OkCommand { get; }

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