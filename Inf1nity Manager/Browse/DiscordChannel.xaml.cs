using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Inf1nity_Manager.Browse
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
            MessagePanel.RequestAddContent += (panel, text) => AddToInputBox(text);
        }

        public SocketTextChannel Channel { private set; get; }

        public async Task Init()
        {
            try
            {
                var msgs = await Channel.GetMessagesAsync().FlattenAsync();
                foreach (var msg in msgs.Reverse())
                    await MessagePanel.AddMessage(msg).ConfigureAwait(false);

                var client = (Application.Current.MainWindow as MainWindow).Bot.Client;
                var guildUser = Channel.Guild.Users.First(e => e.Id == client.CurrentUser.Id);
                var guildPerms = guildUser.GuildPermissions;
                var channelUserPerms = Channel.GetPermissionOverwrite(client.CurrentUser);

                if (Channel.PermissionOverwrites.All(e => e.Permissions.SendMessages == PermValue.Allow))
                {
                    Input.IsEnabled = true;
                    return;
                }

                var rolesPerms = Channel.PermissionOverwrites.Where(r => r.TargetType == PermissionTarget.Role);
                var botRoles = guildUser.Roles;

                bool? roleCanSendMessage = null;
                bool userCanSendMessage = false;

                foreach (var r in botRoles)
                {
                    var ow = Channel.GetPermissionOverwrite(r);
                    if (ow is OverwritePermissions perm)
                    {
                        if (perm.SendMessages == PermValue.Allow)
                        {
                            roleCanSendMessage = true;
                            break;
                        }
                    }
                }

                if (roleCanSendMessage == false)
                {
                    if (channelUserPerms.HasValue && channelUserPerms.Value.SendMessages.HasFlag(PermValue.Allow))
                        userCanSendMessage = true;
                    else if (channelUserPerms.HasValue && channelUserPerms.Value.SendMessages.HasFlag(PermValue.Inherit) && guildPerms.SendMessages)
                        userCanSendMessage = true;
                    else if (!channelUserPerms.HasValue && roleCanSendMessage.HasValue)
                        userCanSendMessage = roleCanSendMessage.Value;
                    else if (!channelUserPerms.HasValue && !roleCanSendMessage.HasValue)
                        userCanSendMessage = false;
                    else
                        userCanSendMessage = false;
                }
                else if (roleCanSendMessage.HasValue)
                    userCanSendMessage = roleCanSendMessage.Value;

                Input.IsEnabled = userCanSendMessage;
            }
            catch(Exception ex)
            {
                MessagePanel.Content = new TextBlock
                {
                    Text = ex.Message,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                MessagePanel.Background = (Brush)new BrushConverter().ConvertFromString("#FFF0F0");

                Input.IsEnabled = false;
            }
        }

        private async void CommandProcessor_CommandRun(object sender, string e)
        {
            if (!string.IsNullOrWhiteSpace(e))
                await Channel.SendMessageAsync(e);
        }

        public void AddMessage(IMessage msg)
        {
            MessagePanel.AddMessage(msg);
        }

        public void AddToInputBox(string text)
        {
            Input.Buffer += text;
            Input.Focus();
        }
    }
}
