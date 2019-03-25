using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Inf1nity.Commands;

namespace Inf1nity
{
    public partial class DiscordBot
    {
        public CommandService AdminCommands = new CommandService();
        public CommandService UserCommands = new CommandService();
        IServiceProvider Services = new ServiceCollection().BuildServiceProvider();

        private async void RegisterCommands()
        {
            if (!CommandsRegistered)
            {
                // Register Admin commands
                await AdminCommands.AddModuleAsync(typeof(AdminCommands), Services);
                Client.MessageReceived += HandleAdminCommand;
                
                // Register help modules
                await AdminCommands.AddModuleAsync(typeof(HelpModule), Services);
                
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
    }
}
