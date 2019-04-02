using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Inf1nity_Manager.Tools;

namespace Inf1nity_Manager.Browse
{
    /// <summary>
    /// Interaction logic for DiscordUser.xaml
    /// </summary>
    public partial class DiscordUser : UserControl
    {
        public DiscordUser(IUser user)
        {
            InitializeComponent();
            User = user;
        }

        public event EventHandler<string> RequestAddContent;

        private IUser _user;
        public IUser User
        {
            set
            {
                _user = value;
                Init(User);
            }
            get => _user;
        }

        public IGuild Guild { private set; get; }

        public void Init(IUser user)
        {
            Icon.ImageSource = Tools.ImageTool.GetImageSource(user.GetAvatarUrl());

            string fullName = user.Username + '#' + user.Discriminator;
            if (user is IGuildUser guildUser && !string.IsNullOrWhiteSpace(guildUser.Nickname))
            {
                Nickname.Text = guildUser.Nickname;
                FullName.Text = fullName;
            }
            else
            {
                Nickname.Text = fullName;
                Nickname.VerticalAlignment = VerticalAlignment.Center;
                FullName.SetValue(VisibilityProperty, Visibility.Hidden);
            }


        }

        private void CopyUserID_Click(object sender, RoutedEventArgs e)
        {
            User.CopyID();
        }

        private void CopyMention_Click(object sender, RoutedEventArgs e)
        {
            User.CopyMention();
        }

        private void Mention_Click(object sender, RoutedEventArgs e)
        {
            RequestAddContent?.Invoke(this, User.Mention);
        }
    }
}
