using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MeloncherAvalonia.ViewModels.Dialogs;
using ReactiveUI;

namespace MeloncherAvalonia.Views.Dialogs
{
	public class AccountsWindow : ReactiveWindow<AccountsViewModel>
	{
		public AccountsWindow()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
			this.WhenActivated(d => d(ViewModel.ShowAddAccountDialog.RegisterHandler(DoShowAddAccountDialogAsync)));
			this.WhenActivated(d => d(ViewModel.ShowAddMicrosoftAccountDialog.RegisterHandler(DoShowAddMicrosoftAccountDialogAsync)));
			this.WhenActivated(d => d(ViewModel.OkCommand.Subscribe(Close)));
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private async Task DoShowAddAccountDialogAsync(InteractionContext<AddAccountViewModel, AddAccountData?> interaction)
		{
			var dialog = new AddAccountWindow();
			dialog.DataContext = interaction.Input;
			var result = await dialog.ShowDialog<AddAccountData?>(this);
			interaction.SetOutput(result);
		}

		private async Task DoShowAddMicrosoftAccountDialogAsync(InteractionContext<AddMicrosoftAccountViewModel, string?> interaction)
		{
			var dialog = new AddMicrosoftAccountWindow();
			dialog.DataContext = interaction.Input;
			var result = await dialog.ShowDialog<string?>(this);
			interaction.SetOutput(result);
		}

		private void Button_OnClick(object? sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}