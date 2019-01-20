using Inf1nity.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.API;
using Discord.WebSocket;

namespace Inf1nity
{
    public class DiscordBot : IHasOutput, INotifyPropertyChanged
    {
        public DiscordBot(string token)
        {
            Token = token;
        }

        #region Properties & Event

        public string Token { private set; get; }

        private bool _running;
        public bool Running
        {
            private set
            {
                _running = value;
                NotifyPropertyChanged();
            }
            get => _running;
        }

        private bool _connected;
        public bool Connected
        {
            private set
            {
                _connected = value;
                NotifyPropertyChanged();
            }
            get => _connected;
        }

        public DiscordSocketClient Client { private set; get; } = new DiscordSocketClient();
        public DiscordSocketConfig Configuration { set; get; } = new DiscordSocketConfig();

        #endregion

        #region Main Methods

        public async void Start()
        {
            Running = true;

            Client.Log += HandleOutput;
            Client.MessageReceived += HandleOutput;
            Client.Ready += Client_Ready;
            Client.Connected += Client_Connected;
            Client.Disconnected += Client_Disconnected;

            await Client.LoginAsync(TokenType.Bot, Token);
            await Client.StartAsync();
        }

        public async void Stop()
        {
            await Client.StopAsync();

            Running = false;
        }

        private Task Client_Ready()
        {
            // TODO: add ready options
            return Task.CompletedTask;
        }

        #endregion

        #region Commands

        public void RunCommand(string command)
        {
            // TODO: Run command options
        }

        #endregion

        #region HandleOutput

        private void HandleOutput(string text) => Output?.Invoke(this, text);

        private Task HandleOutput(LogMessage arg)
        {
            HandleOutput($"{arg.Source}/{arg.Severity}|{arg.Message}");
            return Task.CompletedTask;
        }

        private Task HandleOutput(SocketMessage arg)
        {
            if (arg.Channel is SocketGuildChannel guildChannel)
                HandleOutput($"{guildChannel.Guild.Name}/#{arg.Channel}/{arg.Author}/{arg.Content}");
            else if (arg.Channel is SocketGroupChannel groupChannel)
                HandleOutput($"{groupChannel.Name}/{arg.Author}/{arg.Content}");
            else
            {
                HandleOutput($"{arg.Author}/{arg.Content}");
                System.Diagnostics.Debug.WriteLine("Warning: Message lacks a source!");
            }
            return Task.CompletedTask;
        }

        #endregion

        #region Connection Notification

        private Task Client_Connected()
        {
            Connected = true;
            return Task.CompletedTask;
        }

        private Task Client_Disconnected(Exception arg)
        {
            Connected = false;
            return Task.CompletedTask;
        }

        #endregion

        #region Interface Implementation

        public event EventHandler<string> Output;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            if (PropertyName != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion
    }
}
