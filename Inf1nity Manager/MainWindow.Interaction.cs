using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Inf1nity_Manager
{
    public partial class MainWindow
    {
        #region File Tab

        private void AboutButton(object sender, RoutedEventArgs e) => new Windows.AboutBox().ShowDialog();

        private void OpenCrashLogs(object sender, RoutedEventArgs e)
        {
            var dir = Directory.GetCurrentDirectory() + @"\crashlog.log";
            if (File.Exists(dir))
                Process.Start(dir);
        }

        private void Close(object sender, RoutedEventArgs e) => Close();

        #endregion

        #region Bot Tab

        private void BotStart(object sender = null, RoutedEventArgs e = null)
        {
            if (!Bot?.Running ?? true)
            {
                ConfigureBot();
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
