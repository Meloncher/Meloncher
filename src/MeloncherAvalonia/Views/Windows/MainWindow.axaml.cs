using Avalonia;
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
	}
}