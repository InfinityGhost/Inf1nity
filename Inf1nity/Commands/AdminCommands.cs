using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inf1nity.Commands
{
    public class AdminCommands : ModuleBase
    {
        #region Local methods and properties

        public static int Delay = 2500;

        public async void CommandResponse(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                var replyMessage = await ReplyAsync(message);
                await Task.Delay(Delay);
                await replyMessage.DeleteAsync();
            }
        }

        #endregion

        [Command("del"), Summary("Deletes messages.")]
        public async Task Delete([Remainder, Summary("Amount of messages")] int count)
        {
            await Context.Message.DeleteAsync();
            string response = null;

            var permissions = (Context.User as SocketGuildUser).GuildPermissions;
            if (permissions.Administrator || permissions.ManageMessages)
            {
                var messages = await Context.Channel.GetMessagesAsync(count).FlattenAsync();

                messages.DeleteAll(Context);

                if (messages.Count() > 0)
                    response = $"Deleted {messages.Count()} messages.";
            }
            else
            {
                response = "Error: You lack permissions to manage messages.";
            }

            CommandResponse(response);
        }

        [Command("del"), Summary("Deletes messages.")]
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
                CommandResponse("Error: You lack permissions to manage messages.");
        }

        [Command("announce"), Summary("Announces a string of text, mentions @everyone.")]
        public async Task Announce([Remainder, Summary("The text to announce")] string announcement)
        {
            await Context.Message.DeleteAsync();
            var permissions = (Context.User as SocketGuildUser).GuildPermissions;
            if (permissions.Administrator || permissions.MentionEveryone)
                await ReplyAsync("**Announcement** @everyone " + Environment.NewLine + announcement);
        }

        [Command("vote"), Summary("Creates a vote, mentions @everyone.")]
        public async Task Vote([Remainder, Summary("The text to vote on.")] string content)
        {
            await Context.Message.DeleteAsync();
            var permissions = (Context.User as SocketGuildUser).GuildPermissions;
            if (permissions.Administrator || permissions.MentionEveryone)
            {
                var voteMessage = await ReplyAsync("**Vote** @everyone" + Environment.NewLine + content);
                await voteMessage.AddReactionsAsync(new Emoji[]
                {
                    new Emoji("👍"),
                    new Emoji("👎"),
                });
            }
        }

        [Command("newrole"), Summary("Allows you to create a new role.")]
        public async Task NewRole([Remainder, Summary("Role name.")] string roleName)
        {
            await Context.Message.DeleteAsync();
            var permissions = (Context.User as SocketGuildUser).GuildPermissions;
            if (permissions.Administrator || permissions.ManageRoles)
            {
                var role = await Context.Guild.CreateRoleAsync(name: roleName, color: new Color(255, 255, 255));
                CommandResponse($"Role `{role.Name}` created.");
            }
        }
    }
}
