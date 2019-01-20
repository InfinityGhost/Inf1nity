using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for Console.xaml
    /// </summary>
    public partial class Console : UserControl, INotifyPropertyChanged
    {
        public Console()
        {
            InitializeComponent();
        }

        #region Properties & Events

        public event EventHandler<string> Updated;

        public string Prefix => DateTime.Now.ToLongTimeString() + ": ";

        private string _buffer;
        public string Buffer
        {
            set
            {
                _buffer = value;
                NotifyPropertyChanged();
            }
            get => _buffer;
        }

        public bool IsEmpty => string.IsNullOrWhiteSpace(Buffer);

        #endregion

        #region Methods

        public void Log(object sender, string text) => Log(text);
        public void Log(object content)
        {
            if (!IsEmpty)
                Buffer += Environment.NewLine;
            Buffer += Prefix + content;
            Debug.WriteLine(Prefix + content);
            Updated?.Invoke(this, content.ToString());
        }

        #endregion

        #region Context Menu

        public void ClearConsole(object sender = null, EventArgs e = null) => Buffer = string.Empty;
        public void CopyConsole(object sender = null, EventArgs e = null) => Clipboard.SetText(Buffer);

        private void ShowContextMenu(object sender, MouseButtonEventArgs e) => ContextMenu.IsOpen = true;

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
