using Greet.DataStructureV3.Interfaces;
using Greet.DataStructureV3.ResultsStorage;
using Greet.Plugins.SplitContributions.Buisness.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Greet.Plugins.SplitContributions.Buisness
{
    /// <summary>
    /// A vertex in the graph, contains all the assumptions and calculated results
    /// This class is called process, but it should better be understood as a reference to a process model at a specific location in a pathway, this is what we called in GREET a process reference or vertex
    /// </summary>
    class Process
    {
        #region private members
        /// <summary>
        /// List of inputs that have a flow, copied over and simplified from the process model
        /// </summary>
        List<PInput> _inputs = new List<PInput>();
        /// <summary>
        /// List of outputs that either have a flow or are displaced, copied over and simplified from the process model
        /// </summary>
        List<POutput> _outputs = new List<POutput>();
        /// <summary>
        /// Process model used by that vertex (a vertex is process reference in a specific pathway)
        /// </summary>
        int _processModelId;
        /// <summary>
        /// Unique ID for that vertex in the GREET database
        /// </summary>
        Guid _vertexID;
        /// <summary>
        /// Set to true if this was the first process the Crawler went onto
        /// </summary>
        bool _isStartingProcess;
        /// <summary>
        /// Name of the process model used for that vertex
        /// </summary>
        String _name;
        /// <summary>
        /// The amounts of all processes that this process requires well to product
        /// </summary>
        double _energyUse;
        double _totalInputs;
        double _totalOutputs;


        #endregion

        #region public accessors
        public Process(string name, Guid vertexID)
        {
            _name = name;
            _vertexID = vertexID;
        }
        /// <summary>
        /// Unique ID for that vertex in the GREET database
        /// </summary>
        public Guid VertexID
        {
            get { return _vertexID; }
            set { _vertexID = value; }
        }
        /// <summary>
        /// Name of the process model used for that vertex
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// List of inputs that have a flow, copied over and simplified from the process model
        /// </summary>
        public List<PInput> Inputs
        {
            get { return _inputs; }
            set { _inputs = value; }
        }
        /// <summary>
        /// List of outputs that either have a flow or are displaced, copied over and simplified from the process model
        /// </summary>
        public List<POutput> Outputs
        {
            get { return _outputs; }
            set { _outputs = value; }
        }
        /// <summary>
        /// Process model used by that vertex (a vertex is process reference in a specific pathway)
        /// </summary>
        public int ProcessModelId
        {
            get { return _processModelId; }
            set { _processModelId = value; }
        }
        public bool IsStartingProcess
        {
            get { return _isStartingProcess; }
            set { _isStartingProcess = value; }
        }
        public double EnergyUse
        {
            get { return _energyUse; }
            set { _energyUse = value; }
        }
        public double TotalOutputs
        {
            get { return _totalOutputs; }
            set { _totalOutputs = value; }
        }
        public double TotalInputs
        {
            get { return _totalInputs; }
            set { _totalInputs = value; }
        }
        #endregion
    }
}
