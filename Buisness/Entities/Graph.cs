﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Greet.Plugins.SplitContributions.Buisness
{
    /// <summary>
    /// Contains an oriented graph made of processes and flows
    /// </summary>
    public class Graph
    {
        List<Process> _processes;
        List<Flow> _flows;

        /// <summary>
        /// Tests if a process with the same ID is already inserted in the graph
        /// and if not, inserts the given process to the graph
        /// </summary>
        /// <param name="p"></param>
        internal void AddProcess(Process p)
        {
            if (!_processes.Any(item => item.VertexID == p.VertexID))
                _processes.Add(p);
            else
                throw new Exception("An process with the same vertexID is already in this graph");
        }

        /// <summary>
        /// Tests if a flow with the same start and end points is already inserted in the graph
        /// and if not, inserts the given flow to the graph
        /// </summary>
        /// <param name="f"></param>
        internal void AddFlow(Flow f)
        {
            if(!_processes.Any(item => item.Equals(f)))
                _flows.Add(f);
            else
                throw new Exception("An flow with the same start and end point is already in this graph");
        }
    }
}
