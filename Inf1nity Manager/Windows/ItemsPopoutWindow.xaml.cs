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
        public ItemsPopoutWindow() => InitializeComponent();

        public ItemsPopoutWindow(UIElement content) : this()
        {
            this.Content = content;
        }

        public ItemsPopoutWindow(IEnumerable<UIElement> items) : this()
        {
            this.Content = new ItemsControl { ItemsSource = items };
        }

        public ItemsPopoutWindow(IEnumerable<string> lines) : this()
        {
            var items = lines.ToList().ConvertAll(line =>
            {
                var menuItem = new MenuItem { Header = line };
                menuItem.Click += (i, args) => Clipboard.SetText(line);
                return menuItem;
            });
            this.Content = new ItemsControl { ItemsSource = items };
        }

        public ItemsPopoutWindow(UIElement content, string title) : this(content)
        {
            this.Title = title;
        }

        public ItemsPopoutWindow(IEnumerable<UIElement> items, string title) : this(items)
        {
            this.Title = title;
        }

        public ItemsPopoutWindow(IEnumerable<string> items, string title) : this(items)
        {
            this.Title = title;
        }
    }
}
