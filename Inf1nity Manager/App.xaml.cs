using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Inf1nity_Manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {   
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += GlobalUnhandledException;
        }
        
        private void GlobalUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = default;
            ex = (Exception)e.ExceptionObject;

            var crashDump = new List<string>
            {
                $"-------",
                $"Exception occured at {DateTime.Now}",
                $"Source: {ex.Source}",
                $"Message: {ex.Message}",
                $"HelpLink: {ex.HelpLink}",
                $"StackTrace: {Environment.NewLine}{ex.StackTrace}",
                $"TargetSite: {ex.TargetSite.Name}",
                $"HResult: {ex.HResult}",
            };

            System.IO.File.AppendAllLines(System.IO.Directory.GetCurrentDirectory() + "\\dump" + ".log", crashDump);
        }
    }
}
