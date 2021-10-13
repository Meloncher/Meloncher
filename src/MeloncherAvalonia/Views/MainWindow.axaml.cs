using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MeloncherAvalonia.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;

namespace MeloncherAvalonia.Views
{
	public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
	{
		public MainWindow()
		{
			InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
			this.WhenActivated(d => d(ViewModel.ShowDialog.RegisterHandler(DoShowDialogAsync)));
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private async Task DoShowDialogAsync(InteractionContext<AddAccountViewModel, AddAccountData?> interaction)
		{
			var dialog = new AddAccountWindow();
			dialog.DataContext = interaction.Input;
			var result = await dialog.ShowDialog<AddAccountData?>(this);
			interaction.SetOutput(result); 
		}
	}
}
