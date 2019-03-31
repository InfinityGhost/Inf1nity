using Discord;
using Discord.WebSocket;
using System;
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
            }
            catch(Exception ex)
            {
                MessagePanel.Content = new TextBlock
                {
                    Text = ex.Message,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                MessagePanel.Background = (Brush)new BrushConverter().ConvertFromString("#FFF0F0");
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
