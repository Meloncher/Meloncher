using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MeloncherAvalonia.ViewModels.Dialogs;
using ReactiveUI;

namespace MeloncherAvalonia.Views.Dialogs
{
	public class SettingsWindow : ReactiveWindow<SettingsViewModel>
	{
		public SettingsWindow()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
			this.WhenActivated(d => d(ViewModel.OkCommand.Subscribe(action => Close(action))));
			this.WhenActivated(d => d(ViewModel.ImportCommand.Subscribe(action => Close(action))));
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}