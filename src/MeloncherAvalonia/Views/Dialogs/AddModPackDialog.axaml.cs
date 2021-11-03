using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MeloncherAvalonia.Views.Dialogs
{
	public class AddModPackDialog : UserControl
	{
		public AddModPackDialog()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}