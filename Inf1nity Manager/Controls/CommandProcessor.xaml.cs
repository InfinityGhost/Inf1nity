using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for CommandProcessor.xaml
    /// </summary>
    public partial class CommandProcessor : UserControl, INotifyPropertyChanged
    {
        public CommandProcessor()
        {
            InitializeComponent();
        }

        #region Properties & Events

        public event EventHandler<string> CommandRun;

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

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            if (PropertyName != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion

        private void BufferKeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Enter:
                    {
                        CommandRun?.Invoke(this, Buffer);
                        Buffer = string.Empty;
                        break;
                    }
            }
        }
    }
}
