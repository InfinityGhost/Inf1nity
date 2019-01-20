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
    /// Interaction logic for BotStatus.xaml
    /// </summary>
    public partial class BotStatus : UserControl, INotifyPropertyChanged
    {
        public BotStatus()
        {
            InitializeComponent();
        }

        public bool Active
        {
            set
            {
                SetValue(IsActiveProperty, value);
                NotifyPropertyChanged();
            }
            get => (bool)GetValue(IsActiveProperty);
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            "Active", typeof(bool), typeof(BotStatus));

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
