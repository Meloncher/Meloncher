using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MeloncherAvalonia.Views.Dialogs
{
	public class AccountSelectorDialog : UserControl
	{
		public AccountSelectorDialog()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}