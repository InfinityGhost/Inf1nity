using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Discord.WebSocket;
using Inf1nity_Manager.Extensions;

namespace Inf1nity_Manager.Controls.Items
{
    public class ChannelDictionary : Dictionary<string, ulong>, IXmlSerializable
    {
        public ChannelDictionary() : base() => Clear();

        #region Overrides & Extensions

        public new void Clear()
        {
            base.Clear();
            this.Add(ReplyMessage);
        }
        
        public void Add(SocketTextChannel channel) => Add(channel.Name, channel.Id);
        public void Add(SocketVoiceChannel channel) => Add(channel.Name, channel.Id);
        public void Add(SocketGroupChannel channel) => Add(channel.Name, channel.Id);

        public void Remove(SocketTextChannel channel) => Remove(channel.Name);
        public void Remove(SocketVoiceChannel channel) => Remove(channel.Name);
        public void Remove(SocketGroupChannel channel) => Remove(channel.Name);

        public new void Remove(string key)
        {
            if (key != ReplyMessage.Key)
                base.Remove(key);
        }

        #endregion

        #region Static Objects

        public static KeyValuePair<string, ulong> ReplyMessage = new KeyValuePair<string, ulong>("[Reply]", 0);

        #endregion

        #region Serialization Support

        XmlSchema IXmlSerializable.GetSchema() => null;

        private static XmlSerializer KeySerializer = new XmlSerializer(typeof(string));
        private static XmlSerializer ValueSerializer = new XmlSerializer(typeof(ulong));

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("channel");

                reader.ReadStartElement("name");
                string key = (string)KeySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("id");
                ulong value = (ulong)ValueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(key, value);

                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach(string key in Keys)
            {
                writer.WriteStartElement("channel");

                writer.WriteStartElement("name");
                KeySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("id");
                KeySerializer.Serialize(writer, this[key]);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
