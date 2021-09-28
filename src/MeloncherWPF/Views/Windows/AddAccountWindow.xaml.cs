using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
