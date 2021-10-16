using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MeloncherAvalonia.ViewModels;
using ReactiveUI;
using System;

namespace MeloncherAvalonia.Views
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
	}
}
