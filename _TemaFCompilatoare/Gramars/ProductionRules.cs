using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TemaFCompilatoare.Gramars
{
    class ProductionRule : IComparable<ProductionRule>
    {
        #region Members
        public string NonTerminal { get; set; }
        public List<string> Rule { get; set; }
        public List<string> DirectorSymbol { get; set; }
        #endregion
         
        #region Methods
        public bool IsLeftRecursion()
        {
            if (NonTerminal == Rule[0])
                return true;
            return false;
        }
        public List<string> GetRuleFrom(int index, int length)
        {
            return Rule.GetRange(index, length);
        }
        #endregion

        #region OverRide Region
        public override string ToString()
        {
            string str = null;
            str = NonTerminal + " -->";
            if(Rule.Count == 0)
            {
                str = str + " " + "eps\n";
                return str;
            }
            foreach(var item in Rule)
            {
                str = str + " " + item;
            }
            str += "\n";
            return str;
        }
        public int CompareTo(ProductionRule that)
        {
            if (this.NonTerminal.Equals(that.NonTerminal))
            {
                return that.Rule.Count - this.Rule.Count;
            }
            return this.NonTerminal.CompareTo(that.NonTerminal);
        }

        #endregion
    }
}
