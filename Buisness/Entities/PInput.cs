using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Greet.Plugins.SplitContributions.Buisness.Entities
{
    /// <summary>
    /// Simplified version of an input for a vertex/process
    /// </summary>
    class PInput
    {
        #region private members
        /// <summary>
        /// Unique ID for that input in the Graph structure
        /// </summary>
        Guid _id;
        /// <summary>
        /// Source used to feed the flow to that input
        /// </summary>
        int _mixOrPathwayID;
        /// <summary>
        /// Quantity for that input, this information is extracted from the process model
        /// </summary>
        Value _quantity;
        /// <summary>
        /// Source type used to feed the flow to that input
        /// </summary>
        Greet.DataStructureV3.Interfaces.Enumerators.SourceType _source;
        /// <summary>
        /// The resource ID for that input, defines which resource and which phisical properties to assume for that input
        /// </summary>
        int _resourceID;

        
        #endregion

        #region public accessors
        /// <summary>
        /// The resource ID for that input, defines which resource and which phisical properties to assume for that input
        /// </summary>
        public int ResourceID
        {
            get { return _resourceID; }
            set { _resourceID = value; }
        }
        /// <summary>
        /// Unique ID for that input in the Graph structure
        /// </summary>
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// Quantity for that input, this information is extracted from the process model
        /// </summary>
        public Value Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
        /// <summary>
        /// Source type used to feed the flow to that input
        /// </summary>
        public Greet.DataStructureV3.Interfaces.Enumerators.SourceType Source
        {
            get { return _source; }
            set { _source = value; }
        }
        /// <summary>
        /// Source used to feed the flow to that input
        /// </summary>
        public int MixOrPathwayID
        {
            get { return _mixOrPathwayID; }
            set { _mixOrPathwayID = value; }
        }
        #endregion
    }
}
