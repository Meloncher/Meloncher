using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MeloncherAvalonia.UserControls
{
	public class LoadingBar : UserControl
	{
		public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<LoadingBar, string>("Text");
		public static readonly StyledProperty<int> ValueProperty = AvaloniaProperty.Register<LoadingBar, int>("Value");
		public static readonly StyledProperty<bool> HiddenProperty = AvaloniaProperty.Register<LoadingBar, bool>("Hidden");

		public LoadingBar()
		{
			InitializeComponent();
		}

		public string Text
		{
			get => GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public int Value
		{
			get => GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		public bool Hidden
		{
			get => GetValue(HiddenProperty);
			set => SetValue(HiddenProperty, value);
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}