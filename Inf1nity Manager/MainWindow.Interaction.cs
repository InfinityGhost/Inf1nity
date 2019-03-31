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

        private void CommandProcessor_CommandRun(object sender, string e) => Bot?.SendMessage(e);
    }
}
