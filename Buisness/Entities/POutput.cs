﻿using Greet.DataStructureV3.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Greet.Plugins.SplitContributions.Buisness.Entities
{
    /// <summary>
    /// Simplified version of an input for a vertex/process
    /// </summary>
    class POutput
    {
        #region private members
        Guid _id;
        Value _quantity;
        int _resourceID;
        Boolean _isDisplaved = false;
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
        #endregion
    }
}
