using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
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

namespace Inf1nity_Manager.Guild
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
            ChannelFrame.Content = dC;
            ChannelName.Content = '#' + channel.Name;
            await dC.Init().ConfigureAwait(false);
        }

        public void NotifyMessage(IMessage message)
        {
            if (ChannelsPanel.SelectedItem is SocketTextChannel channel && channel.Id == message.Channel.Id)
            {
                var dChannel = ChannelFrame.Content as DiscordChannel;
                dChannel.MessagePanel.AddMessage(message);
            }
        }

        public void NotifyDeleted(ulong id)
        {
            if (ChannelsPanel.SelectedItem is SocketTextChannel ch)
            {
                var dChannel = ChannelFrame.Content as DiscordChannel;
                dChannel.MessagePanel.RemoveMessage(id);
            }
        }
    }
}
