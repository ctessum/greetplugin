﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Greet.Plugins.SplitContributions.Buisness
{
    /// <summary>
    /// Contains the information necessary to link two processes
    /// </summary>
    public class Flow
    {
        #region private members
        Guid _startVertex;
        Guid _startOutput;
        Guid _endVertex;
        Guid _endOutput;
        #endregion

        /// <summary>
        /// Creates a new Flow
        /// </summary>
        /// <param name="startVertex"></param>
        /// <param name="startOutput"></param>
        /// <param name="endVertex"></param>
        /// <param name="endOutput"></param>
        public Flow(Guid startVertex, Guid startOutput, Guid endVertex, Guid endOutput)
        {
            _startVertex = startVertex;
            _startOutput = startOutput;
            _endVertex = endVertex;
            _endOutput = endOutput;
        }

        public override bool Equals(object obj)
        {
            if (obj is Flow)
            {
                Flow o = obj as Flow;
                return o.StartVertex == this.StartVertex
                    && o.StartOutput == this.StartOutput
                    && o.EndVertex == this.EndVertex
                    && o.EndOutput == this.EndOutput;
            }
            return base.Equals(obj);
        }

        #region public accessors
        public Guid StartVertex
        {
            get { return _startVertex; }
            set { _startVertex = value; }
        }

        public Guid StartOutput
        {
            get { return _startOutput; }
            set { _startOutput = value; }
        }

        public Guid EndVertex
        {
            get { return _endVertex; }
            set { _endVertex = value; }
        }

        public Guid EndOutput
        {
            get { return _endOutput; }
            set { _endOutput = value; }
        }
        #endregion
    }
}
