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
    
        String _name;
        Value _quantity;
        Value _previousQuantity;
        #endregion

        #region public accessors

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
        public Value Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
        public Value PreviousQuantity
        {
            get { return _previousQuantity; }
            set { _previousQuantity = value; }
        }
        public bool CheckConverged()
        {
            const double tolerance = 1e-9;
            double a = _quantity.Val;
            double b = _previousQuantity.Val;
            if (a == b || Math.Abs(a-b)/(a+b) < tolerance) {
                return true;
	        }
            return false;
        }
        #endregion
    }
}
