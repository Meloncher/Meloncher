using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MeloncherAvalonia.ViewModels.Windows;
using MeloncherAvalonia.Views.Windows;

namespace MeloncherAvalonia
{
	public class App : Application
	{
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public override void OnFrameworkInitializationCompleted()
		{
			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
				desktop.MainWindow = new MainWindow
				{
					DataContext = new MainViewModel()
				};

			base.OnFrameworkInitializationCompleted();
		}
	}
}