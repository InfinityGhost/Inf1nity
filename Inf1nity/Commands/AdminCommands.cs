using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inf1nity.Commands
{
    public class AdminCommands : ModuleBase
    {
        #region Local methods

        public static int Delay = 2500;

        public async void Reply(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                var replyMessage = await ReplyAsync(message);
                await Task.Delay(Delay);
                await replyMessage.DeleteAsync();
            }
        }

        #endregion

        public async Task Delete([Remainder, Summary("Amount of messages")] int count)
        {
            await Context.Message.DeleteAsync();
            string reply = null;

            var permissions = (Context.User as SocketGuildUser).GuildPermissions;
            if (permissions.Administrator || permissions.ManageMessages)
            {
                var messages = await Context.Channel.GetMessagesAsync(count).FlattenAsync();
                if (messages.Count() > 0)
                    reply = $"Deleted {messages.Count()} messages.";
            }
            else
            {
                reply = "Error: You lack permissions to manage messages.";
            }

            Reply(reply);
        }

        public async Task Delete()
        {
            await Context.Message.DeleteAsync();

            var permissions = (Context.User as SocketGuildUser).GuildPermissions;
            if (permissions.Administrator || permissions.ManageMessages)
            {
                var message = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
                await message.First().DeleteAsync();
            }
            else
                Reply("Error: You lack permissions to manage messages.");
        }
    }
}
