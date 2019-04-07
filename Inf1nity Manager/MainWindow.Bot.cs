using Discord.WebSocket;
using Inf1nity;
using Inf1nity.Tools;
using Inf1nity_Manager.Browse;
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
                Bot.MessageUpdated += (bot, args) => AppDispatcher.Invoke(() => dV.NotifyUpdated(args.Item2, args.Item1));
                BrowseFrame.Child = dV;
            });

            Bot.CurrentGame = 'v' + Information.AssemblyVersion;
        }

        private void Bot_BotMentioned(object sender, SocketMessage e)
        {
            Notifier.Show(e.Author.Username + '#' + e.Author.Discriminator, MessageTools.CleanseMentions(e));
        }
    }
}
