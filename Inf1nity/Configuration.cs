using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Inf1nity.FileHelper.ReadWrite;

namespace Inf1nity
{
    public class Configuration : INotifyPropertyChanged
    {
        public Configuration() { }
        public Configuration(string path) : this() => Load(path);

        #region Properties

        private string Path { set; get; }

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

        #endregion

        #region Management

        public void Load(string path)
        {
            var content = File.ReadAllLines(path);
            Token = content.Fetch("token");
            Path = path;
        }

        public void Save()
        {
            if (!string.IsNullOrWhiteSpace(Path))
                Save(Path);
            else
                throw new Exception("No default path.");
        }

        public void Save(string path)
        {
            File.WriteAllLines(path, new List<string>
            {
                $"token:{Token}",
            });
            Path = path;
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
