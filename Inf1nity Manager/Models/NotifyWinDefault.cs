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
       
        public void Show(string text, string title)
        {
            if (text != null && title != null)
                TrayIcon.ShowBalloon(text, title);
            else
                throw new ArgumentNullException();
        }
    }
}
