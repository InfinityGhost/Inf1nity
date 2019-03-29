﻿using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Inf1nity_Manager.Guild
{
    /// <summary>
    /// Interaction logic for DiscordChannel.xaml
    /// </summary>
    public partial class DiscordChannel : UserControl
    {
        public DiscordChannel(SocketTextChannel textChannel)
        {
            InitializeComponent();
            Channel = textChannel;
        }

        public SocketTextChannel Channel { private set; get; }

        public async Task Init()
        {
            try
            {
                var msgs = await Channel.GetMessagesAsync().FlattenAsync();
                foreach (var msg in msgs.Reverse())
                    await MessagePanel.AddMessage(msg).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                MessagePanel.Content = ex.Message;
                MessagePanel.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }

        private async void CommandProcessor_CommandRun(object sender, string e)
        {
            if (!string.IsNullOrWhiteSpace(e))
                await Channel.SendMessageAsync(e);
        }
    }
}
