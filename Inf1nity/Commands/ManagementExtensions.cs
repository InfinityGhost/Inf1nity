using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inf1nity.Commands
{
    public static class ManagementExtensions
    {
        /// <summary>
        /// Deletes messages in a channel.
        /// </summary>
        /// <param name="messages">The messages to delete</param>
        /// <param name="channel">The channel they are located</param>
        public static async void DeleteAll(this IEnumerable<IMessage> messages, ICommandContext context)
        {
            await (context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
        }
    }
}
