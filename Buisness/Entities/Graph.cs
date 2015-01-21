using System;
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
        List<Process> _processes = new List<Process>();
        List<Flow> _flows = new List<Flow>();

        /// <summary>
        /// Tests if a process with the same ID is already inserted in the graph
        /// and if not, inserts the given process to the graph
        /// </summary>
        /// <param name="p"></param>
        internal bool AddProcess(Process p)
        {
            if (!_processes.Any(item => item.VertexID == p.VertexID))
            {
                _processes.Add(p);
                return true;
            }
            else
                return false;
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

        internal List<Process> Processes
        {
            get { return _processes; }
            set { _processes = value; }
        }
    }
}
