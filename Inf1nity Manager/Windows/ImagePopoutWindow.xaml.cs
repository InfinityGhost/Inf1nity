using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Inf1nity_Manager.Windows
{
    /// <summary>
    /// Interaction logic for ImagePopoutWindow.xaml
    /// </summary>
    public partial class ImagePopoutWindow : Window
    {
        public ImagePopoutWindow(Image image)
        {
            InitializeComponent();
            this.Content = image;
            Title = image.Source.ToString();

            image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (sender as Image).MouseLeftButtonDown -= Image_MouseLeftButtonDown;
            this.Hide();
        }

        private void DeactivateWin(object sender, EventArgs e)
        {
            if (this.Visibility == Visibility.Hidden)
                this.Close();
        }
    }
}
