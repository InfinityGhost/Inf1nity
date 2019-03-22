using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inf1nity_Manager.Models
{
    internal class NotifyWinDefault : INotifier
    {
        internal NotifyWinDefault(TrayIcon trayIcon)
        {
            TrayIcon = trayIcon;
        }

        private TrayIcon TrayIcon;

        public string Text { set; get; }
        public string Title { set; get; }

        public void Show()
        {
            if (Text != null && Title != null)
                TrayIcon.ShowBalloon(Text, Title);
            else
                throw new ArgumentNullException();
        }

        public void Show(string text, string title)
        {
            Text = text;
            Title = title;
            Show();
        }
    }
}
