using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Inf1nity_Manager.Tools
{
    public static partial class DiscordTools
    {
        public static bool CheckIfUrl(this string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var result))
                return result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps;
            else
                return false;
        }

        public static IEnumerable<Inline> GetInlines(this string text)
        {
            if (text == null)
                return null;
            var tokens = text.Split(' ');

            List<Inline> temp = new List<Inline>();
            int tempindex = 0;
            foreach(var token in tokens)
            {
                var inline = token.GetInline();
                if (inline is Run run)
                {
                    if (temp.Count <= tempindex)
                        temp.Add(run);
                    else if (temp[tempindex] is Run)
                        (temp[tempindex] as Run).Text += ' ' + token;
                    else
                        temp.Add(run);
                }
                else if (inline is Hyperlink link)
                {
                    temp.Add(link);
                    tempindex++;
                }
            }
            return temp;
            
        }

        public static Inline GetInline(this string token)
        {
            if (token.CheckIfUrl())
            {
                var hyperlink = new Hyperlink(new Run(token));
                hyperlink.Click += (a, b) => Process.Start(token);
                return hyperlink;
            }
            else
                return new Run(token);
        }
    }
}
