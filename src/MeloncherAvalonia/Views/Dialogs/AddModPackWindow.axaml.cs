using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CmlLib.Core.Version;
using MeloncherAvalonia.ViewModels.Dialogs;
using ReactiveUI;

namespace MeloncherAvalonia.Views.Dialogs
{
	public class AddModPackWindow : ReactiveWindow<AddModPackViewModel>
	{
		public AddModPackWindow()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
			
			this.WhenActivated(disposable =>
			{
				disposable(ViewModel.ShowSelectVersionDialog.RegisterHandler(DoShowSelectVersionDialogAsync));
				ViewModel.OkCommand.Subscribe(action => Close(action));
			});
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
		
		
		private async Task DoShowSelectVersionDialogAsync(InteractionContext<VersionsViewModel, MVersionMetadata?> interaction)
		{
			var dialog = new VersionsWindow();
			dialog.DataContext = interaction.Input;
			var result = await dialog.ShowDialog<MVersionMetadata?>(this);
			interaction.SetOutput(result);
		}

		private void Button_OnClick(object? sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}