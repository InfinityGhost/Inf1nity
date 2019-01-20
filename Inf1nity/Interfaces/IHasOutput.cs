using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inf1nity.Interfaces
{
    public interface IHasOutput
    {
        event EventHandler<string> Output;
    }
}
