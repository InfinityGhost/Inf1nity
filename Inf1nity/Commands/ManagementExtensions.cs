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
        /// <param name="channel">The channel the messages are located</param>
        public static async void DeleteAll(this IEnumerable<IMessage> messages, IMessageChannel channel)
        {
            try
            {
                await (channel as SocketTextChannel).DeleteMessagesAsync(messages);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Failed to delete messages.", "ManagementExtensions[Warning]");
            }
        }
    }
}
