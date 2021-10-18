using System.Threading.Tasks;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using MeloncherAvalonia.ViewModels;
using ReactiveUI;

namespace MeloncherAvalonia.Views
{
	public class MainWindow : ReactiveWindow<MainViewModel>
	{
		public MainWindow()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
			this.WhenActivated(d => d(ViewModel.ShowSelectAccountDialog.RegisterHandler(DoShowSelectAccountDialogAsync)));
			this.WhenActivated(d => d(ViewModel.ShowSelectVersionDialog.RegisterHandler(DoShowSelectVersionDialogAsync)));
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
	}
}