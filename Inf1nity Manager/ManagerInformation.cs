using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Inf1nity_Manager
{
    public class ManagerInformation
    {
        /// <summary>
        /// Version of the program.
        /// </summary>
        public static string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}
