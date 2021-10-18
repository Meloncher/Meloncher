using System.Windows;
using System.Windows.Controls;

namespace MeloncherWPF.UserControls
{
	public partial class LoadingBar : UserControl
	{
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(LoadingBar));
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(LoadingBar));
		public static readonly DependencyProperty HiddenProperty = DependencyProperty.Register("Hidden", typeof(bool), typeof(LoadingBar));

		public LoadingBar()
		{
			InitializeComponent();
			//DataContext = this;
		}

		public string Text
		{
			get => (string) GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public int Value
		{
			get => (int) GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public bool Hidden
		{
			get => (bool) GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		//public string Text { get; set; }
		//public int Value { get; set; }
		//public bool Hidden { get; set; }
	}
}