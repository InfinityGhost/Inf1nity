﻿using Discord.WebSocket;
using Inf1nity_Manager.Extensions;
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

        private Dictionary<string, ulong> _channels = new Dictionary<string, ulong>
        {
            { @"{Reply}", 0 }
        };
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
            var keys = channels.ConvertAll(e => e.Name);
            var values = channels.ConvertAll(e => e.Id);
            Channels.AddRange(keys, values);
        }

        public ulong? SelectedChannelID { private set; get; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string PropertyName = "")
        {
            if (PropertyName != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion

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
            if (newIndex <= Channels.Count && newIndex >= -1)
                ChannelsBox.SelectedIndex = newIndex;
        }
    }
}
