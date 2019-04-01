﻿using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Inf1nity_Manager.Browse
{
    /// <summary>
    /// Interaction logic for DiscordGuild.xaml
    /// </summary>
    public partial class DiscordGuild : UserControl
    {
        public DiscordGuild()
        {
            InitializeComponent();
        }
        
        public DiscordGuild(SocketGuild guild) : this()
        {
            Init(guild);
        }

        public SocketGuild Guild { set; get; }

        public void Init(SocketGuild guild)
        {
            var channels = guild.Channels.Where(e => e is SocketTextChannel);
            var sorted = channels.ToList().OrderBy(e => e.Position);
            ChannelsPanel.ItemsSource = sorted;
            ChannelsPanel.SelectionChanged += ChannelsPanel_SelectionChanged;
        }

        private async void ChannelsPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var channel = (ChannelsPanel.SelectedItem as SocketTextChannel);
            await Navigate(channel);
        }

        public async Task Navigate(SocketTextChannel channel)
        {
            var dC = new DiscordChannel(channel);
            ChannelFrame.Child = dC;
            ChannelName.Content = '#' + channel.Name;
            await dC.Init().ConfigureAwait(false);
        }

        public void NotifyMessage(IMessage message)
        {
            if (ChannelsPanel.SelectedItem is SocketTextChannel channel && channel.Id == message.Channel.Id)
            {
                var dChannel = ChannelFrame.Child as DiscordChannel;
                dChannel.MessagePanel.AddMessage(message);
            }
        }

        public void NotifyDeleted(ulong id)
        {
            if (ChannelsPanel.SelectedItem is SocketTextChannel ch)
            {
                var dChannel = ChannelFrame.Child as DiscordChannel;
                dChannel.MessagePanel.RemoveMessage(id);
            }
        }

        public void NotifyUpdated(ulong id, IMessage updatedMessage)
        {
            if (ChannelsPanel.SelectedItem is SocketTextChannel ch)
            {
                var dChannel = ChannelFrame.Child as DiscordChannel;
                dChannel.MessagePanel.UpdateMessage(id, updatedMessage);
            }
        }
    }
}
