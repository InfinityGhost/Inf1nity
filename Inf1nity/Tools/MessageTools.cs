using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inf1nity.Tools
{
    public static class MessageTools
    {
        public static string CleanseMentions(SocketMessage message)
        {
            string final = message.Content;
            foreach (var user in message.MentionedUsers)
                final = final.Replace(user.Mention.TrimFirstChar('!'), string.Empty);
            
            foreach (var role in message.MentionedRoles)
                final = final.Replace(role.Mention.TrimFirstChar('!'), string.Empty);

            return final;
        }

        private static string TrimFirstChar(this string text, char character)
        {
            var index = text.IndexOf(character);
            if (index != 0)
                text = text.Remove(index, 1);
            return text;
        }
    }
}
