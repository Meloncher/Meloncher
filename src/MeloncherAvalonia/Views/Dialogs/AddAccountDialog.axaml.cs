using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MeloncherAvalonia.Views.Dialogs
{
	public class AddAccountDialog : UserControl
	{
		public AddAccountDialog()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}