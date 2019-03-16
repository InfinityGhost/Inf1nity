using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inf1nity_Manager.Controls
{
    /// <summary>
    /// Interaction logic for ChannelPicker.xaml
    /// </summary>
    public partial class ChannelPicker : UserControl, INotifyPropertyChanged
    {
        public ChannelPicker()
        {
            InitializeComponent();
        }

        private Dictionary<string, ulong> _channels;
        public Dictionary<string, ulong> Channels
        {
            set
            {
                _channels = value;
                NotifyPropertyChanged();
            }
            get => _channels;
        }
        
        public void AddChannels(object sender, List<SocketTextChannel> channels)
        {
            var channelIds = channels.ConvertAll(e => e.Id);
            var channelNames = channels.ConvertAll(e => e.Name);
            
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string PropertyName = "")
        {
            if (PropertyName != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion

    }
}
