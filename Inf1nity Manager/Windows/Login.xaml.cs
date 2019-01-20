using Inf1nity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using static Inf1nity_Manager.Windows.FileHelper;

namespace Inf1nity_Manager.Windows
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window, INotifyPropertyChanged
    {
        public Login(Configuration config)
        {
            InitializeComponent();
            Config = config;
            ShowDialog();
        }

        private Configuration _config;
        public Configuration Config
        {
            protected set
            {
                _config = value;
                NotifyPropertyChanged();
            }
            get => _config;
        }

        #region Buttons

        private void AcceptButton(object sender, RoutedEventArgs e) => Close();
        private void CancelButton(object sender, RoutedEventArgs e) => Close();

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                Config.Save();
            }
            catch(Exception)
            {
                Config.Save(LoadFile(Directory.GetCurrentDirectory()));
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            if (PropertyName != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion

    }
}
