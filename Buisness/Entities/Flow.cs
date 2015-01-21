using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Greet.Plugins.SplitContributions.Buisness
{
    /// <summary>
    /// Contains the information necessary to link two processes
    /// </summary>
    class Flow
    {
        Guid startVertex;
        Guid startOutput;
        Guid endVertex;
        Guid endOutput;
    }
}
