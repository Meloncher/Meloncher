using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MeloncherAvalonia.ViewModels;
using ReactiveUI;
using System;

namespace MeloncherAvalonia.Views
{
    public class AddAccountWindow : ReactiveWindow<AddAccountViewModel>
    {
		public AddAccountWindow()
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