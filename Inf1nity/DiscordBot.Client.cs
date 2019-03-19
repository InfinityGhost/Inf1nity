using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Inf1nity
{
    public partial class DiscordBot
    {
        public DiscordSocketClient Client { private set; get; } = new DiscordSocketClient();
        public DiscordSocketConfig Configuration { set; get; } = new DiscordSocketConfig();


        #region Event Handlers

        private Task RegisterEvents()
        {
            Client.Log += HandleOutput;
            Client.MessageReceived += HandleOutput;
            Client.MessageDeleted += Client_MessageDeleted;
            Client.Ready += Client_Ready;
            Client.Connected += Client_Connected;
            Client.Disconnected += Client_Disconnected;

            return Task.CompletedTask;
        }
        
        private Task Client_Ready()
        {
            RegisterCommands();
            return Task.CompletedTask;
        }

        private Task Client_MessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            MessageDeleted?.Invoke(this, arg1.Id);
            return Task.CompletedTask;
        }

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

    }
}
