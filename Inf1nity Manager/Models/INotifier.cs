using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inf1nity_Manager.Models
{
    public interface INotifier
    {
        void Show(string text, string title);
    }
}
