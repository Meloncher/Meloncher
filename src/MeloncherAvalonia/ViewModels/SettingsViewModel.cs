using MeloncherCore.Settings;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		[Reactive] public LauncherSettings LauncherSettings { get; set; }
		public SettingsViewModel(LauncherSettings launcherSettings)
		{
			LauncherSettings = launcherSettings;
		}
		
	}
}