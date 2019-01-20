using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using Inf1nity;

namespace Inf1nity_Manager.Windows
{
    /// <summary>
    /// Interaction logic for AboutBox.xaml
    /// </summary>
    public partial class AboutBox : Window
    {
        public AboutBox()
        {
            InitializeComponent();

            DiscordTag.Content = Information.Discord.Tag;
            Version.Content = Information.AssemblyVersion;
            Website.Content = Information.GitHub;
        }

        #region Menu Buttons

        void CloseButton(object sender, RoutedEventArgs e) => Close();

        #endregion

        #region Discord Tag Context Menu

        void CopyTagButton(object sender, RoutedEventArgs e) => Clipboard.SetText((string)DiscordTag.Content);
        void OpenDevDiscord(object sender, RoutedEventArgs e) => Process.Start(Information.Discord.DevLink);

        #endregion

        #region Version Context Menu

        void ChangeLogButton(object sender, RoutedEventArgs e) => Process.Start(Information.GitHub + "/releases/tag/v" + Version.Content);

        #endregion

        #region Website Context Menu

        void OpenWebsite(object sender, RoutedEventArgs e) => Process.Start((string)Website.Content);

        #endregion

        #region General Event Handlers

        void OpenContextMenu(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Control control)
                if (control.ContextMenu != null)
                    control.ContextMenu.IsOpen = true;
        }

        #endregion

    }
}
