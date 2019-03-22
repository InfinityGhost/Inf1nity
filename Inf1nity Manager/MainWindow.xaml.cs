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

        #region Window Events

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.Updated += StatusBar.Update;

            Debug.Listeners.Add(Console.CreateListener());
            Debug.AutoFlush = true;
            
            Debug.WriteLine("Console added to trace listeners.");

            try
            {
                Config = Configuration.Read(DefaultConfigPath);
                Debug.WriteLine("Loaded defaults.");
            }
            catch
            {
                Config = new Configuration();
                Debug.WriteLine("No defaults found, using an empty config file.");
                Config.Write(DefaultConfigPath);
                new Windows.ConfigurationManager(Config);
            }
            
            await TrayIcon.Initialize();
            TrayIcon.ShowWindow += TrayIcon_ShowWindow;

            if (Config.RunAtStart)
            {
                BotStart();
                if (Config.HideAtStart)
                    this.WindowState = WindowState.Minimized;
            }

            if (File.Exists(DefaultChannelsPath))
                ChannelPicker.LoadChannels(DefaultChannelsPath);
            else
                ChannelPicker.SaveChannels(DefaultChannelsPath);

            switch(WindowsVersionTool.GetWindowsVersion())
            { 
                default:
                    Notifier = new NotifyWinDefault(TrayIcon);
                    break;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Bot?.Stop();
            Hide();

            ChannelPicker.SaveChannels(DefaultChannelsPath);
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

        private INotifier Notifier;
        
        private static string DefaultConfigPath => Directory.GetCurrentDirectory() + "\\default.cfg";
        private static string DefaultChannelsPath => Directory.GetCurrentDirectory() + "\\channels.xml";

        #endregion
        
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
            loginWindow.Closed += (win, args) => Config = loginWindow.Config;
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

        #region Misc.

        private void Login(object sender = null, EventArgs e = null)
        {
            Bot = new DiscordBot(Config.Token);
            Bot.Output += Console.Log;
            Bot.MessageReceived += Bot_MessageReceived;
            Bot.MessageDeleted += Bot_MessageDeleted;
            Bot.MessageUpdated += Bot_MessageUpdated;
            Bot.BotMentioned += Bot_BotMentioned;
        }

        private void Bot_BotMentioned(object sender, SocketMessage e)
        {
            Notifier.Show(e.Author.Username + '#' + e.Author.Discriminator, MessageTools.CleanseMentions(e));
        }

        private void Bot_MessageUpdated(object sender, Tuple<SocketMessage, ulong> data)
        {
            Application.Current.Dispatcher.Invoke(() => DiscordMessagePanel.UpdateMessage(data.Item2, data.Item1));
        }

        private void Bot_MessageReceived(object sender, SocketMessage e)
        {
            Application.Current.Dispatcher.Invoke(() => DiscordMessagePanel.AddMessage(e));

            if (!ChannelPicker.Channels.Values.Contains(e.Channel.Id))
                ChannelPicker.Channels.Add(e.Channel as SocketTextChannel);
        }

        private void Bot_MessageDeleted(object sender, ulong e)
        {
            Application.Current.Dispatcher.Invoke(() => DiscordMessagePanel.RemoveMessage(e));
        }

        #endregion

        #region CommandProcessor

        private void CommandProcessor_CommandRun(object sender, string e) => Bot?.SendMessage(e);

        private void CommandProcessor_MessageSend(object sender, string e)
        {
            if (!string.IsNullOrWhiteSpace(e))
            {
                if (ChannelPicker.SelectedChannelID is ulong id && id != 0)
                {
                    var channel = Bot.Client.GetChannel(id);
                    if (channel is SocketTextChannel textChannel)
                        Bot?.SendMessage(e, textChannel);
                }
                else
                    Bot?.SendMessage(e);
            }
        }

        private void DropItem_Event(object sender, DragEventArgs e)
        {
            // Image must be uploaded
        }

        private void CommandProcessor_KeyPress(object sender, Key e)
        {
            switch (e)
            {
                case Key.Up:
                    ChannelPicker.MoveUp();
                    break;
                case Key.Down:
                    ChannelPicker.MoveDown();
                    break;
            }
        }

        #endregion

        
    }
}
