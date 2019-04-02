using Discord;
using Discord.WebSocket;
using Inf1nity_Manager.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                var userRoles = guildUser.Roles;

                Input.IsEnabled = GetSendPermissions(guildUser, userRoles);

            }
            catch(Exception ex)
            {
                var errorInfo = new TextBlock
                {
                    Text = ex.Message,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                errorInfo.MouseLeftButtonDown += (r, arg) => new ExceptionPopoutWindow(ex).ShowDialog();
                
                MessagePanel.Content = errorInfo;
                MessagePanel.Background = (Brush)new BrushConverter().ConvertFromString("#FFF0F0");

                Input.IsEnabled = false;
            }
        }

        #region Tools

        private bool GetSendPermissions(SocketGuildUser guildUser, IEnumerable<IRole> userRoles)
        {
            bool canMessage = false;

            List<GuildPermissions> guildPerms = Channel.Guild.Roles.ToList().ConvertAll(role => role.Permissions);
            List<OverwritePermissions?> rolesPermOw = userRoles.ToList().ConvertAll(role => Channel.GetPermissionOverwrite(role));
            OverwritePermissions? userPermOw = Channel.GetPermissionOverwrite(guildUser);

            if (userPermOw.HasValue && rolesPermOw.Any(p => p.HasValue)) // User perms & role perms
            {
                var userP = userPermOw.Value;
                var rolesP = userPermOw.Value;
                if (userP.SendMessages.HasFlag(PermValue.Allow) && rolesP.SendMessages.HasFlag(PermValue.Allow)) // Both allow
                    canMessage = true;
                else if (userP.SendMessages.HasFlag(PermValue.Deny) && rolesP.SendMessages.HasFlag(PermValue.Deny)) // Both deny
                    canMessage = false;
                else if (userP.SendMessages.HasFlag(PermValue.Allow) && rolesP.SendMessages.HasFlag(PermValue.Inherit)) // User allow, role inherit
                    canMessage = true;
                else if (userP.SendMessages.HasFlag(PermValue.Deny) && rolesP.SendMessages.HasFlag(PermValue.Inherit)) // User deny, role inherit
                    canMessage = false;
                else if (rolesP.SendMessages.HasFlag(PermValue.Allow) && userP.SendMessages.HasFlag(PermValue.Inherit)) // Role allow, user inherit
                    canMessage = true;
                else if (rolesP.SendMessages.HasFlag(PermValue.Deny) && userP.SendMessages.HasFlag(PermValue.Inherit)) // Role deny, user inherit
                    canMessage = false;
                else if (userP.SendMessages.HasFlag(PermValue.Inherit) && userP.SendMessages.HasFlag(PermValue.Inherit)) // Both inherit
                    canMessage = guildPerms.Any(p => p.SendMessages);
                else
                    canMessage = false;
            }
            else if (userPermOw.HasValue && rolesPermOw.Any(p => !p.HasValue)) // User perms only
            {
                var userP = userPermOw.Value;
                if (userP.SendMessages == PermValue.Allow) // Allow
                    canMessage = true;
                else if (userP.SendMessages == PermValue.Inherit) // Inherit
                    canMessage = guildPerms.Any(p => p.SendMessages);
                else if (userP.SendMessages == PermValue.Deny) // Deny
                    canMessage = false;
                else
                    canMessage = false;
            }
            else if (!userPermOw.HasValue && rolesPermOw.Any(p => p.HasValue)) // Role perms only
            {
                foreach (var r in rolesPermOw)
                {
                    if (r is OverwritePermissions rolesP)
                    {
                        if (rolesP.SendMessages == PermValue.Allow) // Allow
                        {
                            canMessage = true;
                            break;
                        }
                        else if (rolesP.SendMessages == PermValue.Inherit) // Inherit
                        {
                            canMessage = guildPerms.Any(p => p.SendMessages);
                            if (canMessage)
                                break;
                        }
                        else if (rolesP.SendMessages == PermValue.Deny) // Deny
                            canMessage = false;
                        else
                            canMessage = false;
                    }
                }
            }
            else // No special perms
                canMessage = guildPerms.Any(p => p.SendMessages == true);

            return canMessage;
        }


        #endregion

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
