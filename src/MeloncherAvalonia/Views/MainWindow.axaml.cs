using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CmlLib.Core.Auth;
using MeloncherAvalonia.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;

namespace MeloncherAvalonia.Views
{
	public partial class MainWindow : ReactiveWindow<MainViewModel>
	{
		public MainWindow()
		{
			InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
			this.WhenActivated(d => d(ViewModel.ShowSelectAccountDialog.RegisterHandler(DoShowSelectAccountDialogAsync)));
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
	}
}
