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
    public partial class DiscordBot
    {
        public event EventHandler<string> Output;
        public event EventHandler<SocketMessage> MessageReceived;
        public event EventHandler<ulong> MessageDeleted;

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

        #region Input

        public async void SendMessage(string message, SocketTextChannel channel = null, string imageUrl = null)
        {
            Embed embed = null;
            if (imageUrl != null)
                embed = new EmbedBuilder { ImageUrl = imageUrl }.Build();

            if (channel == null && LastMessage != null)
                await LastMessage.Channel.SendMessageAsync(message, embed: embed);
            else if (channel != null)
                await channel.SendMessageAsync(message, embed: embed);
            else
                HandleOutput("Error: No recent message to reply to.");
        }

        public async void SendImage(string imageUrl, SocketTextChannel channel = null)
        {
            if (channel == null && LastMessage != null)
                await LastMessage.Channel.SendFileAsync(imageUrl);
            else if (channel != null)
                await channel.SendFileAsync(imageUrl, null);
            else
                HandleOutput("Error: No recent message to reply to.");
        }

        #endregion

        #region Output

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

        #region Get Channels

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

        #region Connection Notification Event

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
