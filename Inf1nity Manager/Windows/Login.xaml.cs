using Inf1nity;
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

namespace Inf1nity_Manager.Windows
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login(Configuration config)
        {
            InitializeComponent();
            Config = config;
            this.ShowDialog();
        }

        public Configuration Config { private set; get; }

        private void AcceptButton(object sender, RoutedEventArgs e)
        {
            Config.Token = TokenBox.Text;
            Close();
        }

        private void CancelButton(object sender, RoutedEventArgs e) => Close();
    }
}
