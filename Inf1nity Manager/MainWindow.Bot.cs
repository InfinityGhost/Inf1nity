using Discord.WebSocket;
using Inf1nity;
using Inf1nity.Tools;
using Inf1nity_Manager.Guild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Inf1nity_Manager
{
    public partial class MainWindow
    {
        private void ConfigureBot(object sender = null, EventArgs e = null)
        {
            Bot = new DiscordBot(Config.Token);
            Bot.Output += Console.Log;
            Bot.MessageReceived += Bot_MessageReceived;
            Bot.MessageDeleted += Bot_MessageDeleted;
            Bot.MessageUpdated += Bot_MessageUpdated;
            Bot.BotMentioned += Bot_BotMentioned;
            Bot.Ready += Bot_Ready;
        }

        private Dispatcher AppDispatcher => Application.Current.Dispatcher;

        private void Bot_Ready(object sender, DateTime e)
        {
            AppDispatcher.Invoke(() =>
            {
                var dV = new DiscordView(Bot.Client.Guilds.ToList());
                Bot.MessageReceived += (bot, msg) => AppDispatcher.Invoke(() => dV.NotifyMessage(msg));
                Bot.MessageDeleted += (bot, id) => AppDispatcher.Invoke(() => dV.NotifyDeleted(id));
                BrowseFrame.Content = dV;
            });
        }

        private void Bot_BotMentioned(object sender, SocketMessage e)
        {
            Notifier.Show(e.Author.Username + '#' + e.Author.Discriminator, MessageTools.CleanseMentions(e));
        }

        private void Bot_MessageUpdated(object sender, Tuple<SocketMessage, ulong> data)
        {
            AppDispatcher.Invoke(() => DiscordMessagePanel.UpdateMessage(data.Item2, data.Item1));
        }

        private void Bot_MessageReceived(object sender, SocketMessage e)
        {
            AppDispatcher.Invoke(() => DiscordMessagePanel.AddMessage(e));

            if (!ChannelPicker.Channels.Values.Contains(e.Channel.Id))
                ChannelPicker.Channels.Add(e.Channel as SocketTextChannel);
        }

        private void Bot_MessageDeleted(object sender, ulong e)
        {
            AppDispatcher.Invoke(() => DiscordMessagePanel.RemoveMessage(e));
        }
    }
}
