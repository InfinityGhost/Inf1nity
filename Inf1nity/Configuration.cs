using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Inf1nity.Tools;
using static Inf1nity.Tools.ReadWriteHelper;

namespace Inf1nity
{
    [XmlRoot("Inf1nity Configuration", IsNullable = true)]
    public class Configuration : INotifyPropertyChanged
    {
        public Configuration() { }

        #region Properties

        [XmlIgnore]
        public string Path { private set; get; }
        public string Version => "1.1";

        #endregion

        #region Settings

        private string _token = string.Empty;
        public string Token
        {
            set
            {
                _token = value;
                NotifyPropertyChanged();
            }
            get => _token;
        }

        private bool _runatstart = false;
        public bool RunAtStart
        {
            set
            {
                _runatstart = value;
                NotifyPropertyChanged();
            }
            get => _runatstart;
        }

        #endregion

        #region Management

        private static XmlSerializer Serializer = new XmlSerializer(typeof(Configuration));

        public void Save()
        {
            if (!string.IsNullOrWhiteSpace(Path))
                Save(Path);
            else
                throw new Exception("No default path.");
        }

        public void Save(string path)
        {
            TextWriter tw = new StreamWriter(path);
            Serializer.Serialize(tw, this);
            Path = path;
        }

        public static Configuration Read(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var config = (Configuration)Serializer.Deserialize(sr);
                config.Path = path;
                return config;
            }
        }


        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            if (PropertyName != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion
    }
}
