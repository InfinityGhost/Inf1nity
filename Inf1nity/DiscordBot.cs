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
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Inf1nity
{
    public partial class DiscordBot : INotifyPropertyChanged
    {
        public DiscordBot(string token)
        {
            Token = token;
        }

        #region Properties & Events

        public DiscordSocketClient Client { private set; get; } = new DiscordSocketClient();
        public DiscordSocketConfig Configuration { set; get; } = new DiscordSocketConfig();

        public CommandService AdminCommands = new CommandService();
        public CommandService UserCommands = new CommandService();
        IServiceProvider Services = new ServiceCollection().BuildServiceProvider();

        public string Token { private set; get; }

        #endregion

        #region Main Methods

        public async void Start()
        {
            Running = true;

            Client.Log += HandleOutput;
            Client.MessageReceived += HandleOutput;
            Client.MessageDeleted += Client_MessageDeleted;
            Client.Ready += Client_Ready;
            Client.Connected += Client_Connected;
            Client.Disconnected += Client_Disconnected;

            try
            {
                await Client.LoginAsync(TokenType.Bot, Token);
                await Client.StartAsync();
            }
            catch (Discord.Net.HttpException httpex)
            {
                Output?.Invoke(this, "Exception Occured: " + httpex.Message);
                Output?.Invoke(this, "Make sure your token is valid!");
                Stop();
            }
            catch (Exception ex)
            {
                Output?.Invoke(this, ex.ToString());
                Stop();
            }
        }

        private Task Client_MessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            MessageDeleted?.Invoke(this, arg1.Id);
            return Task.CompletedTask;
        }

        public async void Stop()
        {
            await Client.StopAsync();

            Running = false;
        }

        private Task Client_Ready()
        {
            RegisterCommands();

            return Task.CompletedTask;
        }

        #endregion

        #region Commands

        private async void RegisterCommands()
        {
            if (!CommandsRegistered)
            {
                // Register Admin Commands
                await AdminCommands.AddModuleAsync(typeof(Commands.AdminCommands), Services);
                Client.MessageReceived += HandleAdminCommand;
                // All commands registered, disallows re-registering commands
                CommandsRegistered = true;
            }
            else
                HandleOutput("Warning: Bot attempted to re-register commands.");
        }

        private async Task HandleAdminCommand(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message))
                return;
            int argPos = 0;
            if (!message.HasCharPrefix('&', ref argPos))
                return;
            var context = new CommandContext(Client, message);
            var result = await AdminCommands.ExecuteAsync(context, argPos, Services);
            if (!result.IsSuccess)
            {
                await arg.DeleteAsync();
                var reply = await context.Channel.SendMessageAsync("Error: " + result.ErrorReason);
                await Task.Delay(Commands.AdminCommands.Delay);
                await reply.DeleteAsync();
            }
        }

        #endregion

        #region Interface Implementation
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            if (PropertyName != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion
    }
}
