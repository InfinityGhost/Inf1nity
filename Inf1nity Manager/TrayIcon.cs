using Inf1nity;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inf1nity_Manager
{
    public class TrayIcon
    {
        NotifyIcon NotifyIcon = new NotifyIcon();

        public Task Initialize()
        {
            string icon = @"Inf1nity_Manager.Assets.Icon.ico";
            Assembly assembly = Assembly.GetExecutingAssembly();

            NotifyIcon.Icon = new System.Drawing.Icon(assembly.GetManifestResourceStream(icon));
            NotifyIcon.MouseClick += NotifyIcon_Click;
            NotifyIcon.Text = "Inf1nity " + $"v{Information.AssemblyVersion}";

            return Task.CompletedTask;  
        }

        public event EventHandler ShowWindow;

        public bool Visible
        {
            set => NotifyIcon.Visible = value;
            get => NotifyIcon.Visible;
        }

        internal int BalloonTipTimeout { set; get; } = 2500000;

        private void NotifyIcon_Click(object sender, MouseEventArgs e) => ShowWindow?.Invoke(this, null);

        public async void ShowBalloon(string text, string title)
        {
            if (text != null && title != null)
            {
                var currentVisibility = Visible;
                Visible = true;
                NotifyIcon.ShowBalloonTip(BalloonTipTimeout, title, text, ToolTipIcon.Info);
                await Task.Delay(BalloonTipTimeout);
                Visible = currentVisibility;
            }
            else
                throw new ArgumentNullException();
        }
    }
}
