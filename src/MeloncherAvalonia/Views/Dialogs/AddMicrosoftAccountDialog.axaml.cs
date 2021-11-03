using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MeloncherAvalonia.Views.Dialogs
{
	public class AddMicrosoftAccountDialog : UserControl
	{
		public AddMicrosoftAccountDialog()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}