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
            var user = message.Author as IGuildUser;
            KickUserDialog(user);
        }

        public static void BanAuthorDialog(this IMessage message)
        {
            var user = message.Author as IGuildUser;
            BanUserDialog(user);
        }

        public static async void KickUserDialog(this IGuildUser user)
        {
            string msg = $"Are you sure you want to kick {user.Username} from {user.Guild.Name}?";
            string title = "Kick User";
            var result = MessageBox.Show(msg, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                await user.KickAsync();
        }

        public static async void BanUserDialog(this IGuildUser user)
        {
            string msg = $"Are you sure you want to ban {user.Username} from {user.Guild.Name}?";
            string title = "Ban User";
            var result = MessageBox.Show(msg, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                await user.BanAsync();
        }

        public static async void ShowInvites(this IGuild guild)
        {
            try
            {
                var allInvites = await guild.GetInvitesAsync();
                var invites = allInvites.Where(inv => !inv.IsTemporary && !inv.IsRevoked).ToList();

                var urls = invites.ConvertAll(invite => invite.Url);

                var win = new ItemsPopoutWindow(urls);
                win.Title = "Invites";
                win.ShowDialog();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "GetInvites[Critical]");
            }
        }

        public static async void Delete(this IMessage message)
        {
            try
            {
                await message.DeleteAsync();
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex, "Message deletion failed");
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
