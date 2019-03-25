using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Discord;
using Discord.WebSocket;
using Inf1nity_Manager.Tools;
using Inf1nity_Manager.Windows;

namespace Inf1nity_Manager.Controls.Items
{
    /// <summary>
    /// Interaction logic for DiscordMessageControl.xaml
    /// </summary>
    public partial class DiscordMessage : UserControl, INotifyPropertyChanged
    {    
        public DiscordMessage(SocketMessage msg)
        {
            InitializeComponent();
            Message = msg;
        }

        private SocketMessage _msg;
        public SocketMessage Message
        {
            set
            {
                _msg = value;
                InitMessage();
            }
            get => _msg;
        }

        private void InitMessage()
        {
            string author = GetUsernameString(Message.Author);
            string guild = null;
            string channel = null;

            if (Message.Channel is SocketGuildChannel guildChannel)
            {
                guild = guildChannel.Guild.Name;
                channel = guildChannel.Name;
            }
            else if (Message.Channel is SocketGroupChannel groupChannel)
            {
                channel = groupChannel.Name;
            }
            else if (Message.Channel is SocketDMChannel dmChannel)
            {
                foreach(var user in dmChannel.Users)
                {
                    if (string.IsNullOrWhiteSpace(channel))
                        channel += $@"{user.Username}";
                    else
                        channel += $@",{user.Username}";
                }
            }

            string header = string.Empty;

            if (guild != null)
                header += $@"{guild}\";
            if (channel != null)
                header += $@"#{channel}\";
            if (author != null)
                header += $@"@{author}";

            if (Message.EditedTimestamp is DateTimeOffset editedTime)
                header += ' ' + $"[Edited at {editedTime.ToLocalTime()}]";

            Header.Content = header;
            MessageContent.Text = Message.Content;

            Time.Content = Message.Timestamp.ToLocalTime();

            var clientUser = (Application.Current.MainWindow as MainWindow).Bot.Client.CurrentUser;
            if (author == GetUsernameString(clientUser))
                EditButton.IsEnabled = true;
            else
                EditButton.IsEnabled = false;

            System.Diagnostics.Debug.WriteLine(Message.Author, "AuthorOfMessageRecieved");
            System.Diagnostics.Debug.WriteLine((Application.Current.MainWindow as MainWindow).Bot.Client.CurrentUser, "ClientUser");

            Image.Source = ImageTool.GetImageSource(Message.Author.GetAvatarUrl());

            try // Soon to be fixed so this issue never happens
            {
                foreach (var attachment in Message.Attachments)
                {
                    if (attachment.Width != null && attachment.Height != null) // Attachment is an image
                    {
                        var image = ImageTool.CreateImage(attachment.Url);
                        image.AttachContextMenu();

                        image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
                        AttachmentsPanel.Children.Add(image);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = sender as System.Windows.Controls.Image;

            image.MouseLeftButtonDown -= Image_MouseLeftButtonDown;
            AttachmentsPanel.Children.Remove(image);

            var popout = new ImagePopoutWindow(image);
            popout.Closing += (win, args) => popout.Content = null;
            popout.Closed += (win, args) =>
            {
                AttachmentsPanel.Children.Add(image);
                image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
            };
            popout.ShowDialog();
        }

        #region Properties & Events

        public event EventHandler MessageDeleted;
        public event EventHandler<Tuple<SocketMessage, string>> MessageUpdated;

        #endregion

        #region Actions

        private async void Delete_Click(object sender, EventArgs e)
        {
            try { await Message.DeleteAsync(); }
            catch(Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }
                
            MessageDeleted?.Invoke(this, new EventArgs());
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(Message.Content);
        }

        private void CopyMessageID_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(Message.Id.ToString());
        }

        private void CopyUserID_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Message.Author.Id.ToString());
        }

        private void KickUser_Click(object sender, RoutedEventArgs e)
        {
            var guild = (Message.Author as SocketGuildUser).Guild;

            string msg = $"Are you sure you want to kick {Message.Author.Username} from {guild.Name}?";
            string title = "Kick User";
            var result = MessageBox.Show(msg, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                (Message.Author as SocketGuildUser).KickAsync();
        }

        private void BanUser_Click(object sender, RoutedEventArgs e)
        {
            var guild = (Message.Author as SocketGuildUser).Guild;

            string msg = $"Are you sure you want to ban {Message.Author.Username} from {guild.Name}?";
            string title = "Ban User";
            var result = MessageBox.Show(msg, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                (Message.Author as SocketGuildUser).BanAsync();
        }

        private void CopyGuildID_Click(object sender, RoutedEventArgs e)
        {
            var guild = (Message.Author as SocketGuildUser).Guild;
            Clipboard.SetText(guild.Id.ToString());
        }

        private async void GetInvites_Click(object sender, RoutedEventArgs e)
        {
            var guild = (Message.Author as SocketGuildUser).Guild;
            var allInvites = await guild.GetInvitesAsync();
            var invites = allInvites.Where(inv => !inv.IsTemporary && !inv.IsRevoked).ToList();

            var urls = invites.ConvertAll(invite => invite.Url);

            var win = new ItemsPopoutWindow(urls);
            win.Title = "Invites";
            win.ShowDialog();
        }

        private void EditMessage_Click(object sender, RoutedEventArgs e)
        {
            var box = new TextBox
            {
                Text = Message.Content,
                Margin = new Thickness(5),
            };
            var popoutWindow = new ItemsPopoutWindow(box);

            box.KeyDown += (tb, args) =>
            {
                if (args.Key == Key.Enter)
                    popoutWindow.Close();
            };

            popoutWindow.MinWidth = 200;
            popoutWindow.MinHeight = 18;
            popoutWindow.Title = "Editing message...";
            popoutWindow.Closed += (win, args) => MessageUpdated?.Invoke(
                this, new Tuple<SocketMessage, string>(Message, (popoutWindow.Content as TextBox).Text));
            popoutWindow.ShowDialog();
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string PropertyName = "")
        {
            if (PropertyName != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion

        #region Tools

        private static string GetUsernameString(SocketUser user)
        {
            return user.Username + '#' + user.DiscriminatorValue;
        }

        #endregion
    }
}
