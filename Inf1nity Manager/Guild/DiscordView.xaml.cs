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

namespace Inf1nity_Manager.Guild
{
    /// <summary>
    /// Interaction logic for DiscordView.xaml
    /// </summary>
    public partial class DiscordView : UserControl
    {
        public DiscordView()
        {
            InitializeComponent();
        }

        public DiscordView(List<SocketGuild> guilds) : this()
        {
            Guilds = guilds;
            TextInit();
        }

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

        public void TextInit()
        {
            GuildsPanel.DisplayMemberPath = "Name";
            GuildsPanel.ItemsSource = Guilds;
        }

        public void ImageInit()
        {
            var icons = new List<Image>();
            foreach(var guild in Guilds)
            {
                var source = Tools.ImageTool.GetImageSource(guild.IconUrl);
                var image = new Image
                {
                    Source = source,
                    Width = 40,
                    Height = 40,
                    Tag = guild,
                };
            }
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
            GuildFrame.Content = new DiscordGuild(guild);
            return Task.CompletedTask;
        }

        public void NotifyMessage(Discord.IMessage message)
        {
            if (GuildFrame.Content is DiscordGuild guild)
                guild.NotifyMessage(message);
        }

        public void NotifyDeleted(ulong id)
        {
            if (GuildFrame.Content is DiscordGuild guild)
                guild.NotifyDeleted(id);
        }
    }
}
