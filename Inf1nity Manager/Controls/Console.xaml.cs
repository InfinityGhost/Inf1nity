using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

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
            QueueDispatcher.Tick += DispatchQueue;
            QueueDispatcher.Start();
        }

        #region Properties & Events

        public event EventHandler<string> Updated;

        public string Prefix => DateTime.Now.ToLongTimeString() + ": ";

        private string _buffer;
        public string Buffer
        {
            private set
            {
                _buffer = value;
                NotifyPropertyChanged();
            }
            get => _buffer;
        }

        public bool IsEmpty => string.IsNullOrWhiteSpace(Buffer);

        private ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        private ConcurrentQueue<string> Queue
        {
            set
            {
                _queue = value;
                if (Queue.IsEmpty)
                    QueueDispatcher.Stop();
                else
                    QueueDispatcher.Start();
            }
            get => _queue;
        }

        private DispatcherTimer QueueDispatcher { set; get; } 
            = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };

        #endregion

        #region Methods

        public void Log(object sender, string text) => Enqueue(text);
        public void Log(object content) => Enqueue(content.ToString());
        
        #endregion

        #region Queue

        private void Enqueue(string text)
        {
            Queue.Enqueue(Prefix + text);
        }

        private void DispatchQueue(object sender, EventArgs e)
        {
            while(!Queue.IsEmpty)
            {
                if (Queue.TryDequeue(out string line))
                {
                    if (!IsEmpty)
                        Buffer += Environment.NewLine;
                    Buffer += line;
                    Updated?.Invoke(this, line);
                }
            }
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
