using System.Threading.Tasks;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using MeloncherAvalonia.ViewModels.Dialogs;
using MeloncherAvalonia.ViewModels.Windows;
using MeloncherAvalonia.Views.Dialogs;
using ReactiveUI;

namespace MeloncherAvalonia.Views.Windows
{
	public class MainWindow : ReactiveWindow<MainViewModel>
	{
		public MainWindow()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
			this.WhenActivated(disposable =>
			{
				disposable(ViewModel.ShowSelectAccountDialog.RegisterHandler(DoShowSelectAccountDialogAsync));
				disposable(ViewModel.ShowSelectVersionDialog.RegisterHandler(DoShowSelectVersionDialogAsync));
				disposable(ViewModel.ShowSettingsDialog.RegisterHandler(DoShowSettingsDialogAsync));
				disposable(ViewModel.CheckUpdates());
			});
			Closing += (sender, args) =>
			{
				if (ViewModel.IsLaunched) args.Cancel = true;
			};
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private async Task DoShowSelectAccountDialogAsync(InteractionContext<AccountsViewModel, MSession?> interaction)
		{
			var dialog = new AccountsWindow();
			dialog.DataContext = interaction.Input;
			var result = await dialog.ShowDialog<MSession?>(this);
			interaction.SetOutput(result);
		}
		
		private async Task DoShowSelectVersionDialogAsync(InteractionContext<VersionsViewModel, MVersionMetadata?> interaction)
		{
			var dialog = new VersionsWindow();
			dialog.DataContext = interaction.Input;
			var result = await dialog.ShowDialog<MVersionMetadata?>(this);
			interaction.SetOutput(result);
		}

		private async Task DoShowSettingsDialogAsync(InteractionContext<SettingsViewModel, SettingsAction?> interaction)
		{
			var dialog = new SettingsWindow();
			dialog.DataContext = interaction.Input;
			var result = await dialog.ShowDialog<SettingsAction?>(this);
			interaction.SetOutput(result);
		}
	}
}