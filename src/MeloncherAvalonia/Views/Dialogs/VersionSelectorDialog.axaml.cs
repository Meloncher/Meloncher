using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MeloncherAvalonia.Views.Dialogs
{
	public class VersionSelectorDialog : UserControl
	{
		public VersionSelectorDialog()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}