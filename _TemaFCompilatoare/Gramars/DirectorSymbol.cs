using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TemaFCompilatoare.Gramars
{
    /// <summary>
    /// Class thar calculates de director symbol array for a given gramar
    /// </summary>
    class DirectorSymbol
    {
        #region Constructors
        public DirectorSymbol()
        {

        }
        public DirectorSymbol(ref Gramar gramar)
        {
            this.gramar = gramar;
        }
        #endregion

        #region Members
        public Gramar gramar {get; set;}
        #endregion
         
        #region Methods
        #region Public Methods
        /// <summary>
        /// Main method for calculatin the director symbols for the given gramar
        /// </summary>
        public void CalculateDirectorSymblos()
        {
            gramar.terminals.Add("$");
            List<string> list = null;
            int visitLength = gramar.nonTerminals.Count();
            var visited = new bool[visitLength + 2];
            for (var index = 0; index < gramar.productionRules.Count; index++)
            {
                for (var i = 0; i < visitLength; i++)
                {
                    visited[i] = false;
                }
                list = new List<string>();
                this.First(index, ref list, ref visited);
                list = list.Distinct().ToList(); //eliminate duplicates
                gramar.productionRules[index].DirectorSymbol = list;
            }
        }
        /// <summary>
        /// Checks if the gramar is a LL1 gramar
        /// </summary>
        /// <returns>True or false</returns>
        public bool CheckLL1()
        {
            var nt = gramar.productionRules[0].NonTerminal;
            List<string> list = new List<string>();
            foreach (var pr in gramar.productionRules)
            {
                if(nt.Equals(pr.NonTerminal))
                {
                    list.AddRange(pr.DirectorSymbol);
                }
                else
                {
                    nt = pr.NonTerminal;
                    if(list.Count != list.Distinct().Count())
                    {
                        //duplicates exists
                        return false;
                    }
                    list = new List<string>();
                }
            }
            return true;
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="nonterminal"></param>
        /// <param name="pos"></param>
        /// <param name="list"></param>
        /// <param name="visited"></param>
        private void First(string nonterminal, int pos, ref List<string> list, ref bool[] visited)
        {
            if(nonterminal == gramar.startSymbol)
            {
                list.Add("$");
            }
            for (int index = 0; index < gramar.productionRules.Count; index++)
            {
                if (gramar.productionRules[index].NonTerminal == nonterminal)
                {
                    if (gramar.productionRules[index].Rule.Count == 0)
                    {
                        Follow(gramar.productionRules[index].NonTerminal, 0, ref list, ref visited);
                    }
                    if (gramar.terminals.Contains(gramar.productionRules[index].Rule[0])) //if it is terminal
                    {
                        list.Add(gramar.productionRules[index].Rule[0]);
                    }
                    else
                    {
                        First(gramar.productionRules[index].Rule[0], index + 1, ref list, ref visited);
                    }
                }
            }
        }
        private void First(int index, ref List<string> list, ref bool[] visited)
        {
            if (gramar.productionRules[index].Rule.Count == 0)
            {
                Follow(gramar.productionRules[index].NonTerminal, 0, ref list, ref visited);
            }
            else
            {
                if (gramar.IsTerminal(index))
                {
                    list.Add(gramar.productionRules[index].Rule[0]);
                }
                else
                {
                    First(gramar.productionRules[index].Rule[0], 0, ref list, ref visited);
                }
            }
        }
        private void Follow(string nonterminal, int pos, ref List<string> list, ref bool[] visited)
        {
            visited[gramar.nonTerminals.IndexOf(nonterminal)] = true; // vizited nodul;
            //if((gramar.nonTerminals.IndexOf(nonterminal) == gramar.nonTerminals.Count - 1)
            //    || nonterminal.Equals(gramar.startSymbol)) //if it is the last element or start symbol
            //{
            //    list.Add("$");
            //}
            if (nonterminal.Equals(gramar.startSymbol)) //if it's the start symbol
            {
                list.Add("$");
            }
            for (var i = 0; i < gramar.productionRules.Count; i++)
            {
                for (var index = 0; index < gramar.productionRules[i].Rule.Count; index++)
                {
                    if (gramar.productionRules[i].Rule[index] == nonterminal)
                    {
                        try
                        {
                            if (gramar.terminals.Contains(gramar.productionRules[i].Rule[index + 1]))
                            {
                                list.Add(gramar.productionRules[i].Rule[index + 1]);
                            }
                            else
                            {
                                First(gramar.productionRules[i].Rule[index + 1], 0, ref list, ref visited);
                            }
                        }
                        catch (System.ArgumentOutOfRangeException)
                        {
                            if (visited[gramar.nonTerminals.IndexOf(gramar.productionRules[i].NonTerminal)] == false)
                                Follow(gramar.productionRules[i].NonTerminal, index + 1, ref list, ref visited);
                        }
                    }
                }
            }
        }
        #endregion
        #region Overrided Methods
        public override string ToString()
        {
            string str = "";
            var rule = 1;
            foreach(var item in gramar.productionRules)
            {
                str += rule + ") ";
                foreach (var ds in item.DirectorSymbol)
                {
                    str += ds + " ";
                }
                rule++;
                str += "\n";
            }
            return str;
        }
        #endregion
        #endregion
    }
}
