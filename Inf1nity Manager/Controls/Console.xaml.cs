using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        #region Extensions

        public ConsoleTraceListener CreateListener() => new ConsoleTraceListener(this);

        #endregion
    }
}
