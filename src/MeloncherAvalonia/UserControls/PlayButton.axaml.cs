using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MeloncherAvalonia.UserControls
{
	public class PlayButton : UserControl
	{
		public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<PlayButton, string>("Text");
		public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<PlayButton, ICommand>("Command");

		public PlayButton()
		{
			InitializeComponent();
		}

		public string Text
		{
			get => GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public ICommand Command
		{
			get => GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}