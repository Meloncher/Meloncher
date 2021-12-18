using System.Globalization;
using System.Threading;
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
			Thread.CurrentThread.CurrentCulture = new CultureInfo("ru");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru");
			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
				desktop.MainWindow = new MainWindow
				{
					DataContext = new MainViewModel()
				};

			base.OnFrameworkInitializationCompleted();
		}
	}
}