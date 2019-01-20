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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Inf1nity;

namespace Inf1nity_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Window Events

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.Updated += StatusBar.Update;
            try
            {
                Config = new Configuration(DefaultConfigPath);
                Console.Log("Loaded defaults.");
            }
            catch
            {
                Config = new Configuration();
                System.Diagnostics.Debug.WriteLine("No defaults found, using an empty config file.");
                System.Diagnostics.Process.Start(DefaultConfigPath);
            }

            await TrayIcon.Initialize();
            TrayIcon.ShowWindow += TrayIcon_ShowWindow;

            if (Config.RunAtStart)
                BotStart();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Bot?.Stop();
            Hide();
            Environment.Exit(0x0);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Minimized:
                    {
                        ShowInTaskbar = false;
                        TrayIcon.Visible = true;
                        Hide();
                        break;
                    }
                case WindowState.Normal:
                    {
                        ShowInTaskbar = true;
                        TrayIcon.Visible = false;
                        break;
                    }
            }
        }

        #endregion

        #region Properties

        private DiscordBot _bot;
        public DiscordBot Bot
        {
            set
            {
                _bot = value;
                NotifyPropertyChanged();
            }
            get => _bot;
        }

        private Configuration _config;
        public Configuration Config
        {
            set
            {
                _config = value;
                NotifyPropertyChanged();
            }
            get => _config;
        }

        private string DefaultConfigPath = Directory.GetCurrentDirectory() + "\\default.cfg";

        #endregion

        private void CommandProcessor_CommandRun(object sender, string e)
        {
            Bot?.RunCommand(e);
        }

        private void Login(object sender = null, EventArgs e = null)
        {
            Bot = new DiscordBot(Config.Token);
            Bot.Output += Console.Log;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            if (PropertyName != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion

        #region File MenuItem

        private void AboutButton(object sender, RoutedEventArgs e)
        {
            new Windows.AboutBox().ShowDialog();
        }

        private void Close(object sender, RoutedEventArgs e) => Close();

        #endregion

        #region Bot MenuItem

        private void BotStart(object sender = null, RoutedEventArgs e = null)
        {
            if (!Bot?.Running ?? true)
            {
                Login();
                Bot?.Start();
            }
            else
                Bot?.Stop();
        }

        private void ConfigButton(object sender, RoutedEventArgs e)
        {
            var loginWindow = new Windows.ConfigurationManager(Config);
            loginWindow.Closed += HandleLoginClose;
        }

        private void HandleLoginClose(object sender, EventArgs e)
        {
            Config = (sender as Windows.ConfigurationManager).Config;
        }

        #endregion

        #region Tray Icon

        private void TrayIcon_ShowWindow(object sender, EventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
        }

        private TrayIcon TrayIcon = new TrayIcon();

        #endregion
    }
}
