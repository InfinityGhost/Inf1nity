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
            InitMessage();
        }

        public SocketMessage Message { private set; get; }

        private void InitMessage()
        {
            string author = Message.Author.Username;
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

            Header.Content = header;
            MessageContent.Text = Message.Content;            
        }

        #region Properties & Events

        public event EventHandler MessageDeleted;

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

        #endregion

        #region Tools



        #endregion

        #region Animation

        

        #endregion

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
