﻿using System;
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
    public class Configuration : INotifyPropertyChanged
    {
        public Configuration() { }

        #region Properties

        private string Path { set; get; }
        public string Version => "2.0";

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
                if (!value)
                    HideAtStart = value;
                NotifyPropertyChanged();
            }
            get => _runatstart;
        }

        private bool _hideAtStart = false;
        public bool HideAtStart
        {
            set
            {
                _hideAtStart = value;
                NotifyPropertyChanged();
            }
            get => _hideAtStart;
        }
        
        #endregion

        #region Management

        private static XmlSerializer Serializer = new XmlSerializer(typeof(Configuration));

        public static Configuration Read(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var config = (Configuration)Serializer.Deserialize(sr);
                config.Path = path;
                return config;
            }
        }

        public void Write()
        {
            using (TextWriter tw = new StreamWriter(Path))
                Serializer.Serialize(tw, this);
        }

        public void Write(string path)
        {
            Path = path;
            Write();
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
