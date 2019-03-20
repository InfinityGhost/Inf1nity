﻿using Discord.WebSocket;
using Inf1nity_Manager.Controls.Items;
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

namespace Inf1nity_Manager.Controls
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

        #region Public Methods

        public void AddMessage(SocketMessage message)
        {
            var msgCtrl = new DiscordMessage(message);
            MessagePanel.Children.Add(msgCtrl);
            ScrollViewer.ScrollToBottom();

            msgCtrl.MessageDeleted += (msg, args) => RemoveMessage(msgCtrl);
        }

        public void UpdateMessage(ulong id, SocketMessage newMessage)
        {
            
            foreach (var child in MessagePanel.Children)
            {
                if (child is DiscordMessage message && message.Message.Id == id)
                {
                    RemoveMessage(id);
                    AddMessage(newMessage);
                    break;
                }
            }
        }
        
        public void RemoveMessage(ulong id)
        {
            foreach (var child in MessagePanel.Children)
            {
                if (child is DiscordMessage message && message.Message.Id == id)
                {
                    RemoveMessage(message);
                    break;
                }
            }
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
