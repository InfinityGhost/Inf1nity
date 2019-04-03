using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using Inf1nity_Manager.Tools;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows;

namespace Inf1nity_Manager.Browse
{
    /// <summary>
    /// Interaction logic for DiscordView.xaml
    /// </summary>
    public partial class DiscordView : UserControl, INotifyPropertyChanged
    {
        public DiscordView()
        {
            InitializeComponent();
        }

        public DiscordView(List<SocketGuild> guilds) : this()
        {
            Guilds = guilds;
        }

        #region Properties

        List<SocketGuild> _guilds;
        public List<SocketGuild> Guilds
        {
            set
            {
                _guilds = value;
                TextInit();
            }
            get => _guilds;
        }

        public static readonly DependencyProperty UserPanelOpenProperty = DependencyProperty.Register(
            "UserPanelOpen", typeof(bool), typeof(DiscordView), new PropertyMetadata(false));
        public bool UserPanelOpen
        {
            set
            {
                SetValue(UserPanelOpenProperty, value);
                NotifyPropertyChanged();
            }
            get => (bool)GetValue(UserPanelOpenProperty);
        }

        #endregion

        public void TextInit()
        {
            GuildsPanel.ItemsSource = Guilds;
        }

        public void ImageInit()
        {
            var icons = new List<Image>();
            foreach(var guild in Guilds)
            {
                var source = ImageTool.GetImageSource(guild.IconUrl);
                var image = new Image
                {
                    Source = source,
                    Width = 40,
                    Height = 40,
                    Tag = guild,
                };
            }
            GuildsPanel.DisplayMemberPath = "";
            GuildsPanel.ItemsSource = icons;
        }

        private async void GuildsPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GuildsPanel.SelectedItem is Image image)
                await Navigate(image.Tag as SocketGuild).ConfigureAwait(false);
            if (GuildsPanel.SelectedItem is SocketGuild guild)
                await Navigate(guild).ConfigureAwait(false);
        }

        public Task Navigate(SocketGuild guild)
        {
            var dGuild = new DiscordGuild(guild);

            dGuild.Expander.IsExpanded = UserPanelOpen;
            GuildFrame.Child = dGuild;
            var bind = new Binding("UserPanelOpen") { Source = dGuild };
            SetBinding(UserPanelOpenProperty, bind);

            return Task.CompletedTask;
        }

        private DiscordGuild SelectedGuild => GuildFrame.Child as DiscordGuild;

        public void NotifyMessage(Discord.IMessage message) => SelectedGuild?.NotifyMessage(message);
        public void NotifyDeleted(ulong id) => SelectedGuild?.NotifyDeleted(id);
        public void NotifyUpdated(ulong id, Discord.IMessage updatedMessage) => SelectedGuild?.NotifyUpdated(id, updatedMessage);

        private void GetGuildInvites_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var guild = GuildsPanel.SelectedItem as Discord.IGuild;
            guild.ShowInvites();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string PropertyName = "")
        {
            if (PropertyName != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion

    }
}
