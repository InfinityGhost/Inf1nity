using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Inf1nity_Manager.Windows
{
    internal class ExceptionPopoutWindow : ItemsPopoutWindow
    {
        public ExceptionPopoutWindow(Exception ex)
        {
            var err = new TextBlock
            {
                Text = ex.ToString(),
                Margin = new Thickness(5),
            };
            err.MouseLeftButtonDown += (a, s) => Clipboard.SetText(ex.ToString());

            var sV = new ScrollViewer
            {
                Content = err,
                MaxHeight = 720,
                MaxWidth = 1280,
            };

            this.Content = sV;
            this.Title = ex.Message;
        }
    }
}
