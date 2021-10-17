using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Windows.Input;

namespace MeloncherAvalonia.UserControls
{
	public partial class PlayButton : UserControl
	{
		public PlayButton()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<PlayButton, string>("Text");
		public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<PlayButton, ICommand>("Command");
		//public static readonly StyledProperty<bool> IsEnabledProperty = AvaloniaProperty.Register<PlayButton, bool>("IsEnabled");

		public string Text
		{
			get { return GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public ICommand Command
		{
			get { return GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		//public bool IsEnabled
		//{
		//	get { return GetValue(IsEnabledProperty); }
		//	set { SetValue(CommandProperty, value); }
		//}
	}
}
