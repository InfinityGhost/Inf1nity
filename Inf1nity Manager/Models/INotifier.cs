using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inf1nity_Manager.Models
{
    public interface INotifier
    {
        string Text { set; get; }
        string Title { set; get; }

        void Show();
    }
}
