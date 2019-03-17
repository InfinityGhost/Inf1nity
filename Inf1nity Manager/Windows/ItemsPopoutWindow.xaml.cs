using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Inf1nity_Manager.Windows
{
    /// <summary>
    /// Interaction logic for ItemsPopoutWindow.xaml
    /// </summary>
    public partial class ItemsPopoutWindow : Window
    {
        public ItemsPopoutWindow(UIElement content)
        {
            InitializeComponent();
            this.Content = content;
        }

        public ItemsPopoutWindow(IEnumerable<UIElement> items)
        {
            InitializeComponent();
            this.Content = new ItemsControl { ItemsSource = items };
        }

        public ItemsPopoutWindow(IEnumerable<string> lines)
        {
            InitializeComponent();
            var items = lines.ToList().ConvertAll(line =>
            {
                var menuItem = new MenuItem { Header = line };
                menuItem.Click += (i, args) => Clipboard.SetText(line);
                return menuItem;
            });
            this.Content = new ItemsControl { ItemsSource = items };
        }
    }
}
