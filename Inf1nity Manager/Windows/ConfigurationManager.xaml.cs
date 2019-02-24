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
    public partial class ConfigurationManager : Window, INotifyPropertyChanged
    {
        public ConfigurationManager(Configuration config)
        {
            InitializeComponent();
            Config = config;
        }

        public ConfigurationManager(Configuration config, bool hideClose) : this(config)
        {
            if (hideClose)
            {
                this.Loaded += (sender, owo) =>
                {
                    ButtonsPanel.Children.Remove(CloseButtonObj);
                    SaveButtonObj.Click -= SaveButton;
                    SaveButtonObj.Click += CloseButton;
                };
            }
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

        private void CloseButton(object sender, RoutedEventArgs e) => Close();

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                Config.Save();
            }
            catch(Exception)
            {
                if (SaveFile(Directory.GetCurrentDirectory(), out string path))
                    Config.Save(path);
            }
            Close();
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

        private void BannerLeftClick(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://discordapp.com/developers/applications");
        }
    }
}
