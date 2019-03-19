using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace Inf1nity
{
    public partial class DiscordBot : INotifyPropertyChanged
    {
        public DiscordBot(string token)
        {
            Token = token;
        }

        public string Token { private set; get; }

        public async void Start()
        {
            Running = true;

            await RegisterEvents().ConfigureAwait(false);

            try
            {
                await Client.LoginAsync(TokenType.Bot, Token);
                await Client.StartAsync().ConfigureAwait(false);
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

        public async void Stop()
        {
            await Client.StopAsync();
            Running = false;
        }

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
