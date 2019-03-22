using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inf1nity_Manager.Tools
{
    internal static class WindowsVersionTool
    {
        public static WindowsVersion GetWindowsVersion()
        {
            var releaseId = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", null);
            Debug.WriteLine(releaseId);
            return WindowsVersion.Old;
        }
    }
}
