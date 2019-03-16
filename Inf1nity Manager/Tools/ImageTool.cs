using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return ImageSourceConverter.ConvertFromString(path) as ImageSource;
        }
    }
}
