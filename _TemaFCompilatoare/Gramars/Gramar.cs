using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TemaFCompilatoare.Gramars
{
    class Gramar
    {
        #region Public Methods
        /// <summary>
        /// Reads the gramar contents from file
        /// </summary>
        /// <param name="filePath"></param>
        public void GetFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            int lineNumber = 0;
            foreach (var line in lines)
            {
                lineNumber++;
                if (lineNumber > 4)
                    break;
                switch (lineNumber)
                {
                    case 1:
                        startSymbol = line;
                        break;
                    case 2:
                        nonTerminals = line.Split(' ').ToList();
                        break;
                    case 3:
                        terminals = line.Split(' ').ToList();
                        break;
                    case 4:
                        int number = 0;
                        int.TryParse(line, out number);
                        
                        break;
                    default:
                        break;
                }
            }
            var Lines = File.ReadAllLines(filePath).Skip(4);
            Lines = Lines.OrderByDescending(o => o).ToList();
            Lines = Lines.OrderBy(o => o).ToList();
            productionRules = new List<ProductionRule>();
            foreach (var item in Lines)
            {
                var aux = item.Split(' ').ToList();
                productionRules.Add(new ProductionRule
                {
                    NonTerminal = aux[0],
                    Rule = aux.GetRange(2, aux.Count - 2)
                });
            }
        }
        /// <summary>
        /// This function generates new nonterminals for the gramar
        /// </summary>
        /// <param name="nonTerminal"></param>
        /// <returns name="newNonTerminal"></returns>
        public string GenerateNonTerminal(string nonTerminal)
        {
            int number = 1;
            string newNT = null;
            bool found = false;
            while (!found)
            {
                newNT = nonTerminal + number;
                found = true;
                foreach (var nt in nonTerminals)
                {
                    if (newNT.Equals(nt))
                    {
                        number++;
                        found = false;
                        break;
                    }
                }
            }
            nonTerminals.Add(newNT);
            return newNT;
        }
        /// <summary>
        /// Main method for the process that removes the rules with the same begining
        /// </summary>
        public void RemoveSameBeging()
        { 
            int n = productionRules.Count;
            for(var index = 0; index < n; index++)
            {
                int start = index, final = -1;
                this.GetPosition_RSM(ref start, ref final, index, n);
                if(final != -1)
                {
                    index = final;
                    int column = -1;
                    this.GetCommonPart_RSM(ref column, start, final);
                    this.ModifyRulesSB_RSM(column, start, final);
                }
            }
            productionRules = productionRules.OrderBy(o => o).ToList();
        }
        /// <summary>
        /// Main method for the process that removes the rules that have left recursion
        /// </summary>
        public void RemoveLeftRecursion()
        {
            int n = productionRules.Count;
            bool foundRecursion = false;
            for(var index = 0; index < n; index++)
            {
                try
                {
                    if (productionRules[index].NonTerminal.Equals(productionRules[index].Rule[0]))
                    {
                        var sindex = index;
                        var nonterminal = productionRules[index].NonTerminal;
                        var newterminal = this.GenerateNonTerminal(nonterminal);
                        var newProdRule = new ProductionRule
                        {
                            NonTerminal = newterminal,
                            Rule = new List<string>()
                        };
                        productionRules.Add(newProdRule);
                        foundRecursion = true;
                        while (foundRecursion)
                        {
                            if (productionRules[sindex].NonTerminal.Equals(productionRules[sindex].Rule[0]))
                            {
                                var alpha = productionRules[sindex].Rule.GetRange(1, productionRules[sindex].Rule.Count - 1);
                                alpha.Add(newterminal);
                                productionRules[sindex].Rule = alpha;
                                productionRules[sindex].NonTerminal = newterminal;
                            }
                            else
                            {
                                productionRules[sindex].Rule.Add(newterminal);
                            }
                            sindex++;
                            if (!productionRules[sindex].NonTerminal.Equals(nonterminal))
                            {
                                foundRecursion = false;
                                index = sindex;
                            }
                        }
                    }
                }
                catch(System.ArgumentOutOfRangeException)
                {
                    //exceptie daca intalneste cazul EPS (rule.count = 0)
                }
            }
            productionRules = productionRules.OrderBy(o => o).ToList();
            nonTerminals = nonTerminals.Distinct().ToList();
        }
        /// <summary>
        /// Verifies if the element at index position in productionRules list is a terminal
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsTerminal(int index)
        {
            try
            {
                if (terminals.Contains(this.productionRules[index].Rule[0]))
                    return true;
            }
            catch (System.ArgumentOutOfRangeException) // if we have rule.count = 0
            {
                return false;
            }
            return false;
        }
        /// <summary>
        /// Generate the matrix for the gramar
        /// </summary>
        public int[,] GenerateMatrix()
        {
            var matrix = new int[nonTerminals.Count, terminals.Count];
            for(int i = 0; i < nonTerminals.Count; i++)
            {
                for(int j = 0; j < terminals.Count; j++)
                {
                    matrix[i, j] = -1;
                }
            }
            
            foreach(var terminal in terminals)
            {
                foreach(var productionRule in productionRules)
                {
                    if (productionRule.DirectorSymbol.Contains(terminal))
                    {
                        matrix[nonTerminals.IndexOf(productionRule.NonTerminal), 
                               terminals.IndexOf(terminal)] = productionRules.IndexOf(productionRule);
                    }
                }
            }
            return matrix;
        }
        #endregion
        #region Private Methods
        #region Rules with same begining
        /// <summary>
        /// This method gets the start and the final positon of the rules that have the same begining
        /// </summary>
        /// <param name="start"> Is the start index</param>
        /// <param name="final"> Is the final index</param>
        /// <param name="index"> Is the current position in the production rules list</param>
        /// <param name="length"> Is the length of the production rules list</param>
        private void GetPosition_RSM(ref int start, ref int final, int index, int length)
        {
            start = index; final = -1;
            for (var ind = index; ind < length; ind++)
            {
                try
                {
                    if (productionRules[ind].Rule[0].Equals(productionRules[ind + 1].Rule[0]) 
                        && productionRules[ind].NonTerminal.Equals(productionRules[ind + 1].NonTerminal)
                        )
                    {
                        final = ind + 1;
                    }
                    else
                    {
                        break;
                    }
                }
                catch (System.ArgumentOutOfRangeException e)
                {
                    //do nothinng (break)
                }
            }
        }
        /// <summary>
        /// Gets the maximum length that match the whole rules
        /// </summary>
        /// <param name="column"> Retein the maximum length </param>
        /// <param name="start"> Is the first index </param>
        /// <param name="final"> Is the last index</param>
        private void GetCommonPart_RSM(ref int column, int start, int final)
        {
            column = 1; //coloana 0 este deja verificata
            bool equal = true;
            try
            {
                while (equal)
                {
                    for (var ind = start; ind < final; ind++)
                    {
                        if (!productionRules[ind].Rule[column].Equals(productionRules[ind + 1].Rule[column]))
                        {
                            equal = false;
                            column--; //pentru ca nu se potriveste acea coloana;
                            break;
                        }
                        else
                        {
                            column++;
                        }
                    }
                }

            }
            catch (System.ArgumentOutOfRangeException e)
            {
                column--;
            }
        }
        /// <summary>
        /// The rules with the same begining are being corrected
        /// </summary>
        /// <param name="column"></param>
        /// <param name="start"></param>
        /// <param name="final"></param>
        private void ModifyRulesSB_RSM(int column, int start, int final)
        {
            List<string> alpha = null;
            column++;
            if (column == 0)
            {
                alpha = productionRules[start].Rule.GetRange(0, column); //retin partea comuna;
            }
            else
            {
                alpha = productionRules[start].Rule.GetRange(0, column);
            }
            var newterminal = this.GenerateNonTerminal(productionRules[start].NonTerminal);
            var oldterminal = productionRules[start].NonTerminal;
            nonTerminals.Add(newterminal); //adaug noul neterminal in gramatica
                                          //modific regulie astfel: daca A->alpha beta atunci o sa defina An->beta;
            for (var ind = start; ind <= final; ind++)
            {
                var newprodRule = new ProductionRule
                {
                    NonTerminal = newterminal,
                    Rule = productionRules[ind].Rule.GetRange(column, productionRules[ind].Rule.Count() - column),
                };
                //daca rule.count() == 0 atunci este EPS
                productionRules[ind] = newprodRule;
            }
            //adaug regula de productie A->alphaA0;
            var prodRule = new ProductionRule
            {
                NonTerminal = oldterminal,
                Rule = alpha,
            };
            prodRule.Rule.Add(newterminal);
            productionRules.Add(prodRule);
        }
        #endregion
        #endregion
        #region Override Region
        /// <summary>
        /// Overrides ToString method from Object class
        /// </summary>
        /// <returns name="string"> The string format for the gramar</returns>
        public override string ToString()
        {
            var str = "Simbol de start: ";
            str += startSymbol + "\n";
            str += "Multimea neterminalelor: ";
            foreach(var item in nonTerminals)
            {
                str += item + " ";
            }
            str += "\nMultimea terminalelor: ";
            foreach(var item in terminals)
            {
                str += item + " ";
            }
            str += "\nMultimea regulilor de productie: \n";
            var rule = 1;
            foreach(var item in productionRules)
            {
                str += rule + ") ";
                rule++;
                str += item.ToString();
            }
            return str;
        }
        #endregion

        #region E,Vn,S,P
        public List<string> nonTerminals { get; set; }
        public List<string> terminals { get; set; }
        public string startSymbol { get; set; }
        public List<ProductionRule> productionRules { get; set; }       
        #endregion
    }
}
