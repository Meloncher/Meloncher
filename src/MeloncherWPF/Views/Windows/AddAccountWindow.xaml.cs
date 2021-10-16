using System.Windows;

namespace MeloncherWPF.Views.Windows
{
	/// <summary>
	/// Логика взаимодействия для AddAccountWindow.xaml
	/// </summary>
	public partial class AddAccountWindow : Window
	{
		public AddAccountWindow()
		{
			InitializeComponent();
		}

		public string ResponsePass
		{
			get { return Pass.Password; }
			set { Pass.Password = value; }
		}
		public string ResponseLogin
		{
			get { return Login.Text; }
			set { Login.Text = value; }
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
