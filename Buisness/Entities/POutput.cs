using Greet.DataStructureV3.ResultsStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Greet.Plugins.SplitContributions.Buisness.Entities
{
    /// <summary>
    /// Simplified version of an output for a vertex/process
    /// </summary>
    class POutput
    {
        #region private members
        /// <summary>
        /// Unique ID for that output in the Graph structure
        /// </summary>
        Guid _id;
        /// <summary>
        /// Quantity for that output, this information is extracted from the process model
        /// </summary>
        Value _quantity;
        /// <summary>
        /// The resource ID for that output, defines which resource and which phisical properties to assume for that output
        /// </summary>
        int _resourceID;
        /// <summary>
        /// <para>Allocated results for that output, only outptuts that are not displaced have upstream allocated to them</para>
        /// <para>These results already account for displaced and allocated amounts. These results are normalized and expressed for 1 unit of the functional unit and are not expressed per Quantity of that output.</para>
        /// </summary>
        Results _results;
        /// <summary>
        /// <para>If true the output is a displaced output, therefore it cannot be used downstream as a feed for another process.</para>
        /// <para>If true no Flow will be created for that output in the Graph.</para>
        /// <para>If false that output is an output to which an upstream has been allocated and can be used for processes downstream.</para>
        /// </summary>
        bool _isDisplaced = false;
        /// <summary>
        /// Displacement ratios for all the products displaced by that output. Only contains somethging if IsDisplaced is true.
        /// DisplacementRatios, DisplacedVertices and DisplacedOutputs are ordered and should be used together
        /// </summary>
        List<double> displacementRatios;
        /// <summary>
        /// Vertices from which the upstream is displaced, this could be for example the us mix when electricity from the grid is displaced or a farm when corn is displaced
        /// DisplacementRatios, DisplacedVertices and DisplacedOutputs are ordered and should be used together
        /// </summary>
        List<Guid> _displacedVertices;
        /// <summary>
        /// OutputID associated with the vertex beeing used for displacedment.
        /// DisplacementRatios, DisplacedVertices and DisplacedOutputs are ordered and should be used together
        /// </summary>
        List<Guid> _displacedOutputs;
        #endregion

        #region public accessors
        /// <summary>
        /// <para>If true the output is a displaced output, therefore it cannot be used downstream as a feed for another process.</para>
        /// <para>If true no Flow will be created for that output in the Graph.</para>
        /// <para>If false that output is an output to which an upstream has been allocated and can be used for processes downstream.</para>
        /// </summary>
        public bool IsDisplaced
        {
            get { return _isDisplaced; }
            set { _isDisplaced = value; }
        }

        /// <summary>
        /// <para>Allocated results for that output, only outptuts that are not displaced have upstream allocated to them</para>
        /// <para>These results already account for displaced and allocated amounts. These results are normalized and expressed for 1 unit of the functional unit and are not expressed per Quantity of that output.</para>
        /// </summary>
        public Results Results
        {
            get { return _results; }
            set { _results = value; }
        }

        /// <summary>
        /// The resource ID for that output, defines which resource and which phisical properties to assume for that output
        /// </summary>
        public int ResourceID
        {
            get { return _resourceID; }
            set { _resourceID = value; }
        }

        /// <summary>
        /// Unique ID for that output in the Graph structure
        /// </summary>
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Quantity for that output, this information is extracted from the process model
        /// </summary>
        public Value Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        /// <summary>
        /// Displacement ratios for all the products displaced by that output. Only contains somethging if IsDisplaced is true.
        /// DisplacementRatios, DisplacedVertices and DisplacedOutputs are ordered and should be used together
        /// </summary>
        public List<double> DisplacementRatios
        {
            get { return displacementRatios; }
            set { displacementRatios = value; }
        }

        /// <summary>
        /// Vertices from which the upstream is displaced, this could be for example the us mix when electricity from the grid is displaced or a farm when corn is displaced
        /// DisplacementRatios, DisplacedVertices and DisplacedOutputs are ordered and should be used together
        /// </summary>
        public List<Guid> DisplacedVertices
        {
            get { return _displacedVertices; }
            set { _displacedVertices = value; }
        }

        /// <summary>
        /// OutputID associated with the vertex beeing used for displacedment.
        /// DisplacementRatios, DisplacedVertices and DisplacedOutputs are ordered and should be used together
        /// </summary>
        public List<Guid> DisplacedOutputs
        {
            get { return _displacedOutputs; }
            set { _displacedOutputs = value; }
        }
        #endregion
    }
}
