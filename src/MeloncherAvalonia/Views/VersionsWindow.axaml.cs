using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MeloncherAvalonia.ViewModels;
using ReactiveUI;

namespace MeloncherAvalonia.Views
{
	public class VersionsWindow : ReactiveWindow<VersionsViewModel>
	{
		public VersionsWindow()
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