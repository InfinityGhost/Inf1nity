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
                try { await replyMessage?.DeleteAsync(); }
                catch (Exception ex) { Debug.WriteLine(ex); }
            }
        }

        #endregion

        [Command("del"), Summary("Deletes messages."), 
            RequireUserPermission(GuildPermission.ManageMessages), 
            RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Delete([Remainder, Summary("Amount of messages")] int count)
        {
            await Context.Message.DeleteAsync();
            var messages = await Context.Channel.GetMessagesAsync(count).FlattenAsync();
            messages.DeleteAll(Context.Channel);
            if (messages.Count() == 1)
                CommandResponse($"Deleted 1 message.");
            if (messages.Count() > 1)
                CommandResponse($"Deleted {messages.Count()} messages.");
            else
                CommandResponse("Error: No messages were found to delete.");
        }

        [Command("del"), Summary("Deletes messages."), 
            RequireUserPermission(GuildPermission.ManageMessages), 
            RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Delete()
        {
            await Context.Message.DeleteAsync();
            var messages = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
            if (messages.ToList().Count == 1)
                await messages.First().DeleteAsync();
            else
                CommandResponse("Error: No recent message to delete.");
        }

        [Command("announce"), Summary("Announces a string of text, mentions @everyone."),
            RequireUserPermission(GuildPermission.MentionEveryone), 
            RequireBotPermission(GuildPermission.MentionEveryone)]
        public async Task Announce([Remainder, Summary("The text to announce")] string announcement)
        {
            await Context.Message.DeleteAsync();
            var permissions = (Context.User as SocketGuildUser).GuildPermissions;
            if (permissions.Administrator || permissions.MentionEveryone)
                await ReplyAsync("**Announcement** @everyone " + Environment.NewLine + announcement);
        }

        [Command("vote"), Summary("Creates a vote, mentions @everyone."), 
            RequireUserPermission(GuildPermission.MentionEveryone), 
            RequireBotPermission(GuildPermission.AddReactions)]
        public async Task Vote([Remainder, Summary("The text to vote on.")] string content)
        {
            var voteMessage = await ReplyAsync("**Vote** @everyone" + Environment.NewLine + content);
            await voteMessage.AddReactionsAsync(new Emoji[]
            {
                new Emoji("👍"),
                new Emoji("👎"),
            });
        }

        [Command("newrole"), Summary("Allows you to create a new role."), 
            RequireUserPermission(GuildPermission.ManageRoles),
            RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task NewRole([Remainder, Summary("Role name.")] string roleName)
        {
            await Context.Message.DeleteAsync();
            var role = await Context.Guild.CreateRoleAsync(name: roleName, color: new Color(255, 255, 255));
            CommandResponse($"Role `{role.Name}` created.");
        }
    }
}
