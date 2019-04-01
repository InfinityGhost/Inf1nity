using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inf1nity_Manager.Browse
{
    /// <summary>
    /// Interaction logic for DiscordMessagePanel.xaml
    /// </summary>
    public partial class DiscordMessagePanel : UserControl
    {
        public DiscordMessagePanel()
        {
            InitializeComponent();
        }

        public event EventHandler<string> RequestAddContent;

        #region Public Methods

        public Task AddMessage(IMessage message)
        {
            var msgCtrl = new DiscordMessage(message);
            MessagePanel.Children.Add(msgCtrl);
            ScrollViewer.ScrollToBottom();

            msgCtrl.RequestAddContent += (msg, text) => RequestAddContent?.Invoke(msg, text);

            if (MessagePanel.Children.Count > 100)
            {
                var oldestMessage = MessagePanel.Children[0];
                MessagePanel.Children.Remove(oldestMessage);
            }
            return Task.CompletedTask;
        }

        public Task UpdateMessage(ulong id, IMessage newMessage)
        {
            foreach (var child in MessagePanel.Children)
            {
                if (child is DiscordMessage message && message.Message.Id == id)
                {
                    message.Message = newMessage;
                    break;
                }
            }
            return Task.CompletedTask;
        }
        
        public Task RemoveMessage(ulong id)
        {
            foreach (var child in MessagePanel.Children)
            {
                if (child is DiscordMessage message && message.Message.Id == id)
                {
                    RemoveMessage(message);
                    break;
                }
            }

            return Task.CompletedTask;
        }

        public void RemoveMessage(DiscordMessage message)
        {
            if (MessagePanel.Children.Contains(message))
                MessagePanel.Children.Remove(message);
            else
                Debug.WriteLine("Message removed that doesn't exist!");
        }

        #endregion

        #region Context Menu

        private void ClearMessages_Click(object sender, RoutedEventArgs e)
        {
            MessagePanel.Children.Clear();
        }

        #endregion
    }
}
