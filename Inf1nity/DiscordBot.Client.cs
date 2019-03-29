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
            Client.MessageUpdated += Client_MessageUpdated;
            Client.Ready += Client_Ready;
            Client.Connected += Client_Connected;
            Client.Disconnected += Client_Disconnected;
            Client.ChannelCreated += Client_ChannelCreated;
            Client.ChannelUpdated += Client_ChannelUpdated;
            Client.ChannelDestroyed += Client_ChannelDestroyed;

            return Task.CompletedTask;
        }

        private Task Client_MessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            MessageUpdated?.Invoke(this, new Tuple<SocketMessage, ulong>(arg2, arg1.Id));
            return Task.CompletedTask;
        }

        private Task Client_Ready()
        {
            RegisterCommands();
            Ready?.Invoke(this, DateTime.Now);
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

        private Task Client_ChannelDestroyed(SocketChannel arg)
        {
            ChannelDeleted?.Invoke(this, arg);
            return Task.CompletedTask;
        }

        private Task Client_ChannelUpdated(SocketChannel arg1, SocketChannel arg2)
        {
            ChannelUpdated?.Invoke(this, new Tuple<SocketChannel, SocketChannel>(arg1, arg2));
            return Task.CompletedTask;
        }

        private Task Client_ChannelCreated(SocketChannel arg)
        {
            ChannelCreated?.Invoke(this, arg);
            return Task.CompletedTask;
        }

        #endregion

    }
}
