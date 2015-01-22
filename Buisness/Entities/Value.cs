using Greet.DataStructureV3.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Greet.Plugins.SplitContributions.Buisness.Entities
{
    /// <summary>
    /// A value and it's unit
    /// </summary>
    public struct Value : IValue
    {
        double _value;
        string _unit;

        public Value(double val, string unit)
        {
            _value = val;
            _unit = unit;
        }

        public Value Copy()
        {
            return new Value(Val,Unit);
        }

        public int SpecieID
        {
            get { throw new NotImplementedException(); }
        }

        public string Unit
        {
            get { return _unit; }
        }

        public double Val
        {
            get { return _value; }
            set { _value = value; }
        }

        double IValue.Value
        {
            get { return _value; }
        }

        public Enumerators.ResultType ValueSpecie
        {
            get { throw new NotImplementedException(); }
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }


        public static Value operator +(Value a, Value b)
        {
            //if (a.Unit == b.Unit)
                return new Value(a.Val + b.Val, a.Unit);
           // else
           //     throw new Exception("Inconsistent units !!!");
        }

        // overload operator * 
        public static Value operator *(Value a, Value b)
        {
            //if (a.Unit == b.Unit)
                return new Value(a.Val * b.Val, a.Unit);
            //else
            //    throw new Exception("Inconsistent units !!!");
        }
        // overload operator / 
        public static Value operator /(Value a, Value b)
        {
            //if (a.Unit == b.Unit)
                return new Value(a.Val / b.Val, a.Unit);
            //else
            //    throw new Exception("Inconsistent units !!!");
        }

    }
}
