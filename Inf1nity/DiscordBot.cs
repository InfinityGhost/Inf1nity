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
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Inf1nity
{
    public class DiscordBot : IHasOutput, INotifyPropertyChanged
    {
        public DiscordBot(string token)
        {
            Token = token;

            AppDomain currentDomain = default;
            currentDomain = AppDomain.CurrentDomain;
            // Handler for unhandled exceptions.
            currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
        }

        #region Properties & Events

        public event EventHandler<SocketMessage> MessageReceived;
        public event EventHandler<ulong> MessageDeleted;

        public DiscordSocketClient Client { private set; get; } = new DiscordSocketClient();
        public DiscordSocketConfig Configuration { set; get; } = new DiscordSocketConfig();

        public CommandService AdminCommands = new CommandService();
        public CommandService UserCommands = new CommandService();
        IServiceProvider Services = new ServiceCollection().BuildServiceProvider();

        public System.Reflection.Assembly GetAssembly()
        {
            return System.Reflection.Assembly.GetExecutingAssembly();
        }

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

        private bool _commandsRegistered;
        public bool CommandsRegistered
        {
            private set
            {
                _commandsRegistered = value;
                NotifyPropertyChanged();
            }
            get => _commandsRegistered;
        }

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

        public void SendMessage(string message, SocketTextChannel channel = null)
        {
            if (channel == null && LastMessage != null)
                LastMessage.Channel.SendMessageAsync(message);
            else if (channel == null && LastMessage == null)
                HandleOutput("Error: No recent message to reply to.");
            else
                channel.SendMessageAsync(message);
        }

        private async void RegisterCommands()
        {
            if (!CommandsRegistered)
            {
                await AdminCommands.AddModuleAsync(typeof(Commands.AdminCommands), Services);
                Client.MessageReceived += HandleAdminCommand;
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

        #region HandleOutput

        public SocketMessage LastMessage { private set; get; }

        private void HandleOutput(string text) => Output?.Invoke(this, text);

        private Task HandleOutput(LogMessage arg)
        {
            HandleOutput($"{arg.Source}/{arg.Severity}|{arg.Message}");
            return Task.CompletedTask;
        }

        private Task HandleOutput(SocketMessage arg)
        {
            LastMessage = arg;
            MessageReceived?.Invoke(this, arg);

            if (arg.Channel is SocketGuildChannel guildChannel)
                HandleOutput($"{guildChannel.Guild.Name}/#{arg.Channel}/@{arg.Author}/{arg.Content}");
            else if (arg.Channel is SocketGroupChannel groupChannel)
                HandleOutput($"{groupChannel.Name}/{arg.Author}/{arg.Content}");
            else if (arg.Channel is SocketDMChannel dMChannel)
                HandleOutput($"{dMChannel.Users}/{arg.Author}/{arg.Content}");
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

        #region Channels

        public SocketTextChannel GetTextChannel(ulong id)
        {
            if (Running)
            {
                var channel = Client.GetChannel(id);
                if (channel is SocketTextChannel textChannel)
                    return textChannel;
                else
                    throw new ArgumentException("Invalid text channel ID.");
            }
            else
                throw new InvalidOperationException("Bot is not running.");
        }

        public List<SocketTextChannel> GetGuildTextChannels(ulong id)
        {
            if (Running)
            {
                var guild = Client.GetGuild(id);
                if (guild != null)
                {
                    var channels = new List<SocketTextChannel>();

                    foreach (SocketTextChannel textChannel in guild.Channels)
                        channels.Add(textChannel);

                    return channels;
                }
                else
                    throw new ArgumentException("Invalid guild ID.");
            }
            else
                throw new InvalidOperationException("Bot is not running.");
        }

        public SocketTextChannel FindTextChannel(string guildName, string channelName)
        {
            if (Running)
            {
                var matchingGuilds = Client.Guilds.Where(g => g.Name == guildName);
                if (matchingGuilds.Count() == 1)
                {
                    var guild = matchingGuilds.First();
                    var matchingChannels = guild.TextChannels.Where(ch => ch.Name == channelName);
                    if (matchingChannels.Count() == 1)
                        return matchingChannels.First();
                    else if (matchingChannels.Count() == 0)
                        throw new ArgumentException($"No channel found by the name of \"{channelName}\".");
                    else
                        throw new ArgumentException($"More than one channel in the guild \"{guild.Name}\" with the name \"{channelName}\".");
                }
                else if (matchingGuilds.Count() == 0)
                    throw new ArgumentException($"No guild found by the name of \"{guildName}\".");
                else
                    throw new ArgumentException($"More than one guild with the name \"{guildName}\".");
                
            }
            else
                throw new InvalidOperationException("Bot is not running.");
        }

        public List<SocketTextChannel> GetAllTextChannels()
        {
            if (Running)
            {
                var textChannels = new List<SocketTextChannel>();
                foreach (var guild in Client.Guilds)
                    foreach (var textChannel in guild.TextChannels)
                        textChannels.Add(textChannel);
                return textChannels;
            }
            else
                throw new InvalidOperationException("Bot is not running.");
        }

        #endregion

        #region Unhandled Exception Handling

        private void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = default;
            ex = (Exception)e.ExceptionObject;

            var crashDump = new List<string>
            {
                $"-------",
                $"Exception occured at {DateTime.Now}",
                $"Source: {ex.Source}",
                $"Message: {ex.Message}",
                $"HelpLink: {ex.HelpLink}",
                $"StackTrace: {Environment.NewLine}{ex.StackTrace}",
                $"TargetSite: {ex.TargetSite.Name}",
                $"HResult: {ex.HResult}",
            };

            System.IO.File.AppendAllLines(System.IO.Directory.GetCurrentDirectory() + "\\crashlog" + ".log", crashDump);
        }

        #endregion

    }
}
