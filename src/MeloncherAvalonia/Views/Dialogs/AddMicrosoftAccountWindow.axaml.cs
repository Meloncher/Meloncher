using System;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MeloncherAvalonia.ViewModels.Dialogs;
using ReactiveUI;

namespace MeloncherAvalonia.Views.Dialogs
{
	public class AddMicrosoftAccountWindow : ReactiveWindow<AddMicrosoftAccountViewModel>
	{
		public AddMicrosoftAccountWindow()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
			this.WhenActivated(d => d(ViewModel.OkCommand.Subscribe(Close)));
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private void Button_OnClick(object? sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}