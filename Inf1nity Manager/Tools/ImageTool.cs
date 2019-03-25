using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Inf1nity_Manager.Tools
{
    public static class ImageTool
    {
        private static ImageSourceConverter ImageSourceConverter = new ImageSourceConverter();

        public static Image CreateImage(string path)
        {
            var image = new Image { Source = GetImageSource(path) };
            return image;
        }

        public static ImageSource GetImageSource(string path)
        {
            if (path != null)
                return ImageSourceConverter.ConvertFromString(path) as ImageSource;
            else
                return null;
        }

        public static void AttachContextMenu(this Image image)
        {
            var openUrlButton = new MenuItem { Header = "Open image" };
            openUrlButton.Click += (btn, args) => System.Diagnostics.Process.Start(image.Source.ToString());

            var copyUrlButton = new MenuItem { Header = "Copy image URL" };
            copyUrlButton.Click += (btn, args) => Clipboard.SetText(image.Source.ToString());

            var items = new List<MenuItem>
            {
                openUrlButton,
                copyUrlButton,
            };

            image.ContextMenu = new ContextMenu { ItemsSource = items };
        }
    }
}
