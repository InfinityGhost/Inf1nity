using Discord;
using Discord.WebSocket;
using Inf1nity_Manager.Controls.Items;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Inf1nity_Manager.Guild
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
            }
            catch(Exception ex)
            {
                MessagePanel.Content = ex.Message;
                MessagePanel.HorizontalAlignment = HorizontalAlignment.Center;
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
