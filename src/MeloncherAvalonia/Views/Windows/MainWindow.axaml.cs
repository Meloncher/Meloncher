using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MeloncherAvalonia.ViewModels.Windows;
using ReactiveUI;

namespace MeloncherAvalonia.Views.Windows
{
	public class MainWindow : ReactiveWindow<MainViewModel>
	{
		public MainWindow()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
			this.WhenActivated(disposable => { disposable(ViewModel.CheckUpdates()); });
			Closing += (sender, args) =>
			{
				if (ViewModel.IsLaunched) args.Cancel = true;
			};
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private void Button_Close_OnClick(object? sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Button_Minimize_OnClick(object? sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}
	}
}