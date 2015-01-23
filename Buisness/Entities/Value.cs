using Greet.DataStructureV3.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Greet.Plugins.SplitContributions.Buisness.Entities
{
    /// <summary>
    /// A value and it's unit, extends from IValue to be compatible with GREET unit conversion methods
    /// </summary>
    public class Value : IValue
    {
        #region private members
        /// <summary>
        /// The numerical value for that value
        /// </summary>
        double _value;
        /// <summary>
        /// The unit associated with the numerical value
        /// </summary>
        string _unit;
        #endregion

        #region methods and constructor
        public Value(double val, string unit)
        {
            _value = val;
            _unit = unit;
        }
        /// <summary>
        /// Returns a new instance of a Value with the same value and unit
        /// </summary>
        /// <returns></returns>
        public Value Copy()
        {
            return new Value(Val, Unit);
        }
        /// <summary>
        /// Returns a new instance of a Value with the same value and unit
        /// </summary>
        /// <returns></returns>
        internal static Value Clone(Value value)
        {
            return new Value(value.Val, value.Unit);
        }
        #endregion

        #region public accessors
        /// <summary>
        /// For compatibility with IValue, returns -1
        /// </summary>
        public int SpecieID
        {
            get { return -1; }
        }
        /// <summary>
        /// The unit associated with the numerical value
        /// </summary>
        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }
        /// <summary>
        /// The numerical value for that value
        /// </summary>
        public double Val
        {
            get { return _value; }
            set { _value = value; }
        }
        /// <summary>
        /// The numerical value for that value
        /// </summary>
        double IValue.Value
        {
            get { return _value; }
        }
        /// <summary>
        /// For compatibility with IValue, returns resource
        /// </summary>
        public Enumerators.ResultType ValueSpecie
        {
            get { return Enumerators.ResultType.resource; }
        }
        /// <summary>
        /// For compatibility with IValue, returns 0
        /// </summary>
        public int CompareTo(object obj)
        {
            return 0;
        }
        #endregion

        #region operators
        /// <summary>
        /// overload operator +
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Value operator +(Value a, Value b)
        {
            if (a.Unit == b.Unit)
                return new Value(a.Val + b.Val, a.Unit);
            else
                throw new Exception("Inconsistent units !!!");
        }

        /// <summary>
        /// overload operator -
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Value operator -(Value a, Value b)
        {
            if (a.Unit == b.Unit)
                return new Value(a.Val - b.Val, a.Unit);
            else
                throw new Exception("Inconsistent units !!!");
        }

        /// <summary>
        /// overload operator *
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Value operator *(Value a, Value b)
        {
            return new Value(a.Val * b.Val, a.Unit);
        }
        
        /// <summary>
        /// overload operator / 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Value operator /(Value a, Value b)
        {
            return new Value(a.Val / b.Val, a.Unit);
        }
        #endregion opertators

        public override string ToString()
        {
            return _value + " " + _unit;
        }
    }
}
