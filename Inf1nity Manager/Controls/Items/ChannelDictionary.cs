using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Discord.WebSocket;
using Inf1nity_Manager.Extensions;

namespace Inf1nity_Manager.Controls.Items
{
    [XmlRoot(elementName: "Channels")]
    public class ChannelDictionary : Dictionary<string, ulong>, IXmlSerializable, INotifyPropertyChanged
    {
        public ChannelDictionary() : base()
        {
            Clear();
            _values = new ValueCollection(this);
            _keys = new KeyCollection(this);
        }

        #region Overrides & Extensions

        public new void Clear()
        {
            base.Clear();
            this.Add(ReplyChannel);
        }
        
        public void Add(SocketTextChannel channel) => Add(GetChannelName(channel), channel.Id);
        public void Add(SocketVoiceChannel channel) => Add(GetChannelName(channel), channel.Id);
        public void Add(SocketGroupChannel channel) => Add(GetChannelName(channel), channel.Id);
        public void Add(SocketDMChannel channel) => Add(GetChannelName(channel), channel.Id);

        public new void Add(string key, ulong value)
        {
            if (!Keys.Contains(key) && !Values.Contains(value))
                base.Add(key, value);
        }

        public void Remove(SocketTextChannel channel) => Remove(GetChannelName(channel));
        public void Remove(SocketVoiceChannel channel) => Remove(GetChannelName(channel));
        public void Remove(SocketGroupChannel channel) => Remove(GetChannelName(channel));
        public void Remove(SocketDMChannel channel) => Remove(GetChannelName(channel));

        public new void Remove(string key)
        {
            if (key != ReplyChannel.Key)
                base.Remove(key);
        }

        #endregion

        public static string GetChannelName(SocketTextChannel channel) => $"{channel.Guild.Name}/#{channel.Name}";
        public static string GetChannelName(SocketVoiceChannel channel) => $"{channel.Guild.Name}/VC:{channel.Name}";
        public static string GetChannelName(SocketGroupChannel channel) => $"GC:{channel.Name}";
        public static string GetChannelName(SocketDMChannel channel) => $"DM:{channel.Users}";

        #region Properties

        private KeyCollection _keys = new KeyCollection(new Dictionary<string, ulong>());
        public new KeyCollection Keys
        {
            set
            {
                _keys = value;
                NotifyPropertyChanged();
            }
            get => _keys;
        }

        private ValueCollection _values;
        public new ValueCollection Values
        {
            set
            {
                _values = value;
                NotifyPropertyChanged();
            }
            get => _values;
        }

        #endregion

        #region Static Objects

        public static KeyValuePair<string, ulong> ReplyChannel = new KeyValuePair<string, ulong>("[Reply]", 0);

        #endregion

        #region Serialization Support

        XmlSchema IXmlSerializable.GetSchema() => null;

        private static XmlSerializer KeySerializer = new XmlSerializer(typeof(string));
        private static XmlSerializer ValueSerializer = new XmlSerializer(typeof(ulong));

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Clear();

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("Channel");

                reader.ReadStartElement("Name");
                string key = (string)KeySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("ID");
                ulong value = (ulong)ValueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                Add(key, value);

                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach(string key in Keys)
            {
                writer.WriteStartElement("Channel");

                writer.WriteStartElement("Name");
                KeySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("ID");
                ValueSerializer.Serialize(writer, this[key]);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
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
