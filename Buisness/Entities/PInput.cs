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
        Guid _id;
        int _mixOrPathwayID;
        Value _quantity;
        Greet.DataStructureV3.Interfaces.Enumerators.SourceType _source;
        int _resourceID;

        
        #endregion

        #region public accessors
        public int ResourceID
        {
            get { return _resourceID; }
            set { _resourceID = value; }
        }
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Value Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
 
        public Greet.DataStructureV3.Interfaces.Enumerators.SourceType Source
        {
            get { return _source; }
            set { _source = value; }
        }

        public int MixOrPathwayID
        {
            get { return _mixOrPathwayID; }
            set { _mixOrPathwayID = value; }
        }
        #endregion
    }
}
