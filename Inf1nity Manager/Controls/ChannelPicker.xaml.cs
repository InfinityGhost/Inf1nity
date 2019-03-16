using Discord.WebSocket;
using Inf1nity_Manager.Controls.Items;
using Inf1nity_Manager.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Xml.Serialization;

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

        #region Channels

        private static XmlSerializer ChannelSerializer = new XmlSerializer(typeof(ChannelDictionary));

        private ChannelDictionary _channelDictionary = new ChannelDictionary();
        public ChannelDictionary Channels
        {
            set
            {
                _channelDictionary = value;
                NotifyPropertyChanged();
            }
            get => _channelDictionary;
        }

        public ulong? SelectedChannelID { private set; get; }

        public void LoadChannels(string path)
        {
            using (var sr = new StreamReader(path))
                Channels = (ChannelDictionary)ChannelSerializer.Deserialize(sr);
        }

        public void SaveChannels(string path)
        {
            using (var sw = new StreamWriter(path))
                ChannelSerializer.Serialize(sw, Channels);
        }

        #endregion

        #region Selection

        private void ChannelsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChannelsBox.SelectedItem is KeyValuePair<string, ulong> item)
                SelectedChannelID = item.Value;
            else
                SelectedChannelID = null;
        }

        private void ChannelsBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var change = -e.Delta / 120;
            var newIndex = ChannelsBox.SelectedIndex + change;
            if (newIndex <= Channels.Count && newIndex >= 0)
                ChannelsBox.SelectedIndex = newIndex;
        }

        #endregion

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
