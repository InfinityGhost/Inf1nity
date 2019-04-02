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

namespace Inf1nity_Manager.Browse
{
    /// <summary>
    /// Interaction logic for DiscordMessageControl.xaml
    /// </summary>
    public partial class DiscordMessage : UserControl, INotifyPropertyChanged
    {    
        public DiscordMessage(IMessage msg)
        {
            InitializeComponent();
            Message = msg;
        }

        private IMessage _msg;
        public IMessage Message
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
                EditedTime.Content = $"[Edited at {editedTime.ToLocalTime()}]";
            else
                EditedTime.SetValue(VisibilityProperty, Visibility.Hidden);

            Header.Content = header;
            MessageContent.Text = Message.Content;
            EditBox.Text = Message.Content;
            EditBox.GotFocus += EditBox_GotFocus;
            EditBox.LostFocus += EditBox_LostFocus;
            EditBox.KeyDown += EditBox_KeyDown;

            Time.Content = Message.Timestamp.ToLocalTime();

            var clientUser = (Application.Current.MainWindow as MainWindow).Bot.Client.CurrentUser;
            if (author == GetUsernameString(clientUser))
                EditButton.IsEnabled = true;
            else
                EditButton.IsEnabled = false;

            Image.Source = ImageTool.GetImageSource(Message.Author.GetAvatarUrl());

            try // Fixed in dev build
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
                System.Diagnostics.Trace.WriteLine(ex, "Message.Attachments.get()[Warning]");
            }
        }

        private void EditBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                        {
                            EditBox.Text += Environment.NewLine; // TODO: fix duplicate inputs
                            EditBox.CaretIndex = EditBox.Text.Length;
                        }
                        else
                            EditBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        break;
                    }
            }
        }

        private void EditBox_GotFocus(object sender, RoutedEventArgs e)
        {
            EditBox.Text = Message.Content;
            MessageContent.SetValue(VisibilityProperty, Visibility.Hidden);
        }


        private void EditBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Edit(EditBox.Text);
            EditBox.SetValue(VisibilityProperty, Visibility.Hidden);
            MessageContent.SetValue(VisibilityProperty, Visibility.Visible);
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

        public event EventHandler<string> RequestAddContent;

        #endregion

        #region Actions

        private void Delete_Click(object sender, EventArgs e)
        {
            Message.Delete();
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            Message.Content.Copy();
        }

        private void CopyMessageID_Click(object sender, EventArgs e)
        {
            Message.CopyID();
        }

        private void CopyUserID_Click(object sender, RoutedEventArgs e)
        {
            Message.Author.CopyID();
        }

        private void CopyMention_Click(object sender, RoutedEventArgs e)
        {
            Message.Author.CopyMention();
        }

        private void Mention_Click(object sender, RoutedEventArgs e)
        {
            RequestAddContent?.Invoke(this, Message.Author.Mention);
        }

        private void KickUser_Click(object sender, RoutedEventArgs e)
        {
            Message.KickAuthorDialog();
        }

        private void BanUser_Click(object sender, RoutedEventArgs e)
        {
            Message.BanAuthorDialog();
        }

        private void CopyGuildID_Click(object sender, RoutedEventArgs e)
        {
            (Message.Author as IGuildUser).Guild.CopyID();
        }

        private void GetGuildInvites_Click(object sender, RoutedEventArgs e)
        {
            var guild = (Message.Author as SocketGuildUser).Guild;
            guild.ShowInvites();
        }

        private void EditMessage_Click(object sender, RoutedEventArgs e)
        {
            EditBox.SetValue(VisibilityProperty, Visibility.Visible);
            EditBox.Focus();
            EditBox.CaretIndex = EditBox.Text.Length;
        }

        public void Edit(string text)
        {
            if (Message.Content != text)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow.Bot.UpdateMessage(Message, text);
            }
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

        private static string GetUsernameString(IUser user)
        {
            return user.Username + '#' + user.DiscriminatorValue;
        }

        #endregion

    }
}
