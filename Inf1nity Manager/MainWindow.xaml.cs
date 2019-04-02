using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using Discord.WebSocket;
using Inf1nity;
using Inf1nity.Tools;
using Inf1nity_Manager.Controls;
using Inf1nity_Manager.Browse;
using Inf1nity_Manager.Models;
using Inf1nity_Manager.Tools;

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

        private INotifier Notifier;
        private TrayIcon TrayIcon = new TrayIcon();

        private static string DefaultConfigPath => Directory.GetCurrentDirectory() + "\\default.cfg";

        private void WriteDebug(string message, string severity) => Trace.WriteLine(message, $"Infinity_Manager[{severity}]");

        #endregion

        #region Window Events

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Update statusbar on console update
            Console.Updated += StatusBar.Update;
            // Hook debug listeners
            Debug.Listeners.Add(Console.CreateListener());
            Debug.AutoFlush = true;

            WriteDebug("Console added to trace listeners.", "Info");

            // Load default configuration
            try
            {
                Config = Configuration.Read(DefaultConfigPath);
                WriteDebug("Loaded defaults.", "Info");
            }
            catch
            {
                Config = new Configuration();
                WriteDebug("No defaults found, using an empty config file.", "Warning");
                Config.Write(DefaultConfigPath);
                new Windows.ConfigurationManager(Config);
            }

            // Tray Icon
            await TrayIcon.Initialize();
            TrayIcon.ShowWindow += (icon, args) =>
            {
                Show();
                WindowState = WindowState.Normal;
            };

            // Run at start if configured
            if (Config.RunAtStart)
            {
                BotStart();
                if (Config.HideAtStart)
                    this.WindowState = WindowState.Minimized;
            }

            Notifier = new NotifyWinDefault(TrayIcon);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Bot?.Stop();
            Hide();
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
                case WindowState.Maximized:
                case WindowState.Normal:
                    {
                        ShowInTaskbar = true;
                        TrayIcon.Visible = false;
                        break;
                    }
            }
        }

        private void Window_DropEvent(object sender, DragEventArgs e)
        {
            // Image must be uploaded
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
