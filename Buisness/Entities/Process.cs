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
    /// </summary>
    class Process
    {
        #region private members
        List<PInput> _inputs = new List<PInput>();
        List<POutput> _outputs = new List<POutput>();
        int _processModelId;
        Guid _vertexID;
        CanonicalProcess _processResults;
        String _name;
        #endregion

        #region public accessors

        public CanonicalProcess ProcessResults
        {
            get { return _processResults; }
            set { _processResults = value; }
        }
        public Guid VertexID
        {
            get { return _vertexID; }
            set { _vertexID = value; }
        }
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public List<PInput> Inputs
        {
            get { return _inputs; }
            set { _inputs = value; }
        }
        public List<POutput> Outputs
        {
            get { return _outputs; }
            set { _outputs = value; }
        }
        public int ProcessModelId
        {
            get { return _processModelId; }
            set { _processModelId = value; }
        }
        #endregion
    }
}
