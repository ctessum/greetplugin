using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Greet.Plugins.SplitContributions.Buisness.Entities
{
    class Amounts
    {
        #region private members
        Dictionary<Guid, double> _amount;
        #endregion

        #region accessors
        public Dictionary<Guid, double> Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }
        #endregion

        #region methods and constructor
        /// <summary>
        /// Initialize a new "Amounts" holder with the vertex ID of the
        /// process it represents.
        /// </summary>
        /// <param name="vertex">The vertex ID of the process that this amount represents</param>
        public Amounts(Guid vertex) {
            _amount = new Dictionary<Guid,double>();
            _amount.Add(vertex, 1);
        }
        public Amounts()
        {
            _amount = new Dictionary<Guid, double>();
        }
        public Dictionary<Guid,double>.Enumerator GetEnumerator(){
            return _amount.GetEnumerator();
        }
        /// <summary>
        /// Multiply another set of amounts by a value and then add it to
        /// the current set.
        /// </summary>
        /// <param name="a">The other set of amounts to add</param>
        /// <param name="x">The value to multiply the other amounts by</param>
        public void AddScaled(Amounts a, double x) {
            foreach (KeyValuePair<Guid, double> item in a)
            {
                double v = 0;
                _amount.TryGetValue(item.Key, out v);
                _amount.Remove(item.Key);
                _amount.Add(item.Key, item.Value * x + v);
            }
        }
        /// <summary>
        /// Check to see if amounts have changed. Note: Always 
        /// use the newer amounts as an argument to the older
        /// amounts, otherwise it won't work correctly.
        /// </summary>
        /// <param name="a">Amounts from the next iteration.</param>
        /// <returns></returns>
        public bool CheckConverged(Amounts a)
        {
            const double tolerance = 1e-9;
            foreach (KeyValuePair<Guid, double> item in a)
            {
                double v = 0;
                _amount.TryGetValue(item.Key,out v);

                if (Math.Abs(item.Value-v)/(item.Value+v) > tolerance) {
                    return false;
	            }
            }
            return true;
        }
        public void Add(Guid key, double value)
        {
            _amount.Add(key, value);
        }
        public Amounts Copy()
        {
            Amounts output = new Amounts();
            foreach (KeyValuePair<Guid, double> item in _amount)
                output.Add(item.Key, item.Value);
            return output;
        }
        public void Reset(Guid vertexID)
        {
            _amount.Clear();
            _amount.Add(vertexID, 1);
        }
        #endregion
    }
}
