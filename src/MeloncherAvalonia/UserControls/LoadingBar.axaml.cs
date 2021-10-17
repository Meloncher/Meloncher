using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MeloncherAvalonia.UserControls
{
	public partial class LoadingBar : UserControl
	{
		public LoadingBar()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<LoadingBar, string>("Text");
		public static readonly StyledProperty<int> ValueProperty = AvaloniaProperty.Register<LoadingBar, int>("Value");
		public static readonly StyledProperty<bool> HiddenProperty = AvaloniaProperty.Register<LoadingBar, bool>("Hidden");

		public string Text
		{
			get { return GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public int Value
		{
			get { return GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		public bool Hidden
		{
			get { return GetValue(HiddenProperty); }
			set { SetValue(HiddenProperty, value); }
		}
	}
}
