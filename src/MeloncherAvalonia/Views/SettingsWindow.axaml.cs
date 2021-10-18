using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MeloncherAvalonia.ViewModels;
using MeloncherCore.Settings;

namespace MeloncherAvalonia.Views
{
	public class SettingsWindow : ReactiveWindow<SettingsViewModel>
	{
		public SettingsWindow()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}