using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MeloncherAvalonia.ViewModels.Dialogs;
using MeloncherAvalonia.ViewModels.Windows;
using MeloncherAvalonia.Views.Dialogs;
using MeloncherCore.ModPack;
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

		private async Task DoShowSettingsDialogAsync(InteractionContext<SettingsViewModel, SettingsAction?> interaction)
		{
			var dialog = new SettingsWindow();
			dialog.DataContext = interaction.Input;
			var result = await dialog.ShowDialog<SettingsAction?>(this);
			interaction.SetOutput(result);
		}
	}
}