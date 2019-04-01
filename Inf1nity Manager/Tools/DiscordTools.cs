using Discord;
using Discord.WebSocket;
using Inf1nity_Manager.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Inf1nity_Manager.Tools
{
    public static class DiscordTools
    {
        public static void Copy(this string value) => Clipboard.SetText(value);

        private static void Copy(this object value) => Clipboard.SetText(value.ToString());

        public static void KickAuthorDialog(this IMessage message)
        {
            var guild = (message.Author as SocketGuildUser).Guild;

            string msg = $"Are you sure you want to kick {message.Author.Username} from {guild.Name}?";
            string title = "Kick User";
            var result = MessageBox.Show(msg, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                (message.Author as SocketGuildUser).KickAsync();
        }

        public static void BanAuthorDialog(this IMessage message)
        {
            var guild = (message.Author as SocketGuildUser).Guild;

            string msg = $"Are you sure you want to ban {message.Author.Username} from {guild.Name}?";
            string title = "Ban User";
            var result = MessageBox.Show(msg, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                (message.Author as SocketGuildUser).BanAsync();
        }

        public static async void ShowInvites(this SocketGuild guild)
        {
            var allInvites = await guild.GetInvitesAsync();
            var invites = allInvites.Where(inv => !inv.IsTemporary && !inv.IsRevoked).ToList();

            var urls = invites.ConvertAll(invite => invite.Url);

            var win = new ItemsPopoutWindow(urls);
            win.Title = "Invites";
            win.ShowDialog();
        }

        public static async void Delete(this IMessage message)
        {
            try
            {
                await message.DeleteAsync();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex, "Message deletion failed");
            }
        }

        public static void CopyMention(this IUser user)
        {
            Copy(user.Mention);
        }

        public static void CopyID(this IMessage message)
        {
            Copy(message.Id);
        }

        public static void CopyID(this IUser user)
        {
            Copy(user.Id);
        }

        public static void CopyID(this IGuild guild)
        {
            Copy(guild.Id);
        }
    }
}
