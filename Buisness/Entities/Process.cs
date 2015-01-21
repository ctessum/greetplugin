using Greet.DataStructureV3.Interfaces;
using Greet.DataStructureV3.ResultsStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Greet.Plugins.SplitContributions.Buisness
{
    /// <summary>
    /// A vertex in the graph, contains all the assumptions and calculated results
    /// </summary>
    class Process
    {
        IProcess _processModel;
        CanonicalProcess _processResults;
        Guid _vertexID;
    }
}
