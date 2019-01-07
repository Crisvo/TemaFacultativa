using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TemaFCompilatoare.Gramars
{
    /// <summary>
    /// Generates C# code for a given gramar
    /// </summary>
    class CodeLL1Generator
    {
        #region Constructor
        public CodeLL1Generator(Gramar gramar)
        {
            _gramar = gramar;
            _matrix = GenerateMatrix();
        }
        #endregion
        
        #region Methods
        /// <summary>
        /// Main function for LL1 code generator
        /// </summary>
        /// <param name="stringName"></param>
        /// <param name="iteratorName"></param>
        /// <returns></returns>
        public string GenerateCode(string stringName = null, string iteratorName = null)
        {
            int nrBrace = 0;
            int nrIf = 0;
            bool[] visit = new bool[_gramar.nonTerminals.Count + 1];
            for(var i = 0; i < _gramar.nonTerminals.Count; i++)
            {
                visit[i] = false;
            }
            #region Code 
            string code = "using System;\n" +
                           "using System.Collections.Generic; \n" +
                           "using System.IO; \n" +
                           "using System.Linq; \n" +
                           "namespace _CompilatoareTFConsoleMain \n" +
                           "{\n" +
                           "class Program\n" +
                           "{\n" +
                            "static List<string> st = null;\n" +
                            "static void Main(string[] args)\n" +
                            "{\n" +
                                "while (true)\n" +
                                "{\n" +
                                   "Console.WriteLine(\"Alegeti medoda prin care sa introduceti propozitia: \"\n" +
                                                     "+ \"\\n\\t1. Intruducere cale fisier.\"\n" +
                                                    "+ \"\\n\\t2. Introducere in consola.\"\n" +
                                                    "+ \"\\n\\t3. EXIT!\");\n" +
                                    "var choice = Console.ReadLine();\n" +
                                    "if (choice.Equals(\"2\"))\n" +
                                    "{\n" +
                                        "Console.WriteLine(\"Introducteti propozitia, astfel:\\n\"\n" +
                                                        "+ \"Pe un singur rand, terminalele fiind separate fiecare prin cate un spatiu.\");\n" +
                                        "var line = Console.ReadLine();\n" +
                                        "st = new List<string>();\n" +
                                        "st = line.Split(' ').ToList();\n" +
                                        "Console.WriteLine(\"Propozitia a fost introdusa!\");\n" +
                                    "}\n" +
                                    "else\n" +
                                    "{\n" +
                                        "if (choice.Equals(\"1\"))\n" +
                                        "{\n" +
                                            "Console.WriteLine(\"Scrieti calea catre fisier: \");\n" +
                                            "var path = Console.ReadLine();\n" +
                                            "if (File.Exists(path))\n" +
                                            "{\n" +
                                                "st = new List<string>();\n" +
                                                "var lines = File.ReadAllLines(path);\n" +
                                                "foreach (var line in lines)\n" +
                                                "{\n" +
                                                    "st.AddRange(line.Split(' ').ToList());\n" +
                                                "}\n" +
                                                "Console.WriteLine(\"Cuvintele au fost citite cu succes!\");\n" +
                                            "}\n" +
                                            "else\n" +
                                            "{\n" +
                                                "Console.WriteLine(\"Fisierul nu exista!\");\n" +
                                                "continue;\n" +
                                            "}\n" +
                                        "}\n" +
                                        "else\n" +
                                        "{\n" +
                                            "if (choice.Equals(\"3\"))\n" +
                                            "{\n" +
                                                "Console.WriteLine(\"Aplicatia consola se va inchide.\\nPress any key...\");\n" +
                                                "Console.Read();\n" +
                                                "return;\n" +
                                            "}\n" +
                                            "else\n" +
                                            "{\n" +
                                                "Console.WriteLine(\"Alegerea { 0}, este incorecta\", choice);\n" +
                                "continue;\n" +
                                "}\n" +
                                    "}\n" +
                                    "}\n" +
                                    "try\n" +
                                    "{\n" +
                                    "    Exec();\n" +
                                    "}\n" +
                                    "catch (Exception exc)\n" +
                                    "{\n" +
                                    "Console.WriteLine(exc.Message);\n" +
                                    "Console.ReadKey();\n" +
                                    "}\n" +
                                    "}\n" +
                                     " }\n";
            code += "static void Exec()\n{\n" +
                    "int poz = 0;\n" +
                    _gramar.startSymbol + "(ref poz);\n" +
                    "if(st[poz] == \"$\")\n{\n" +
                    "Console.WriteLine(\"Propozitia este corecta\");" +
                    "Console.ReadKey();\n}\n" +
                    "else\n{\n" +
                    "Console.WriteLine(\"Propozitia este incorecta\");\n" +
                    "Console.ReadKey();\n}\n" +
                    "}\n";
            #endregion
            foreach(var rule in _gramar.productionRules)
            {
                if(visit[_gramar.nonTerminals.IndexOf(rule.NonTerminal)] == false)
                {
                    if(nrBrace > 0)
                    {
                        while(nrBrace > 0)
                        {
                            code += "\n}\n";
                            nrBrace--;
                        }
                    }
                    visit[_gramar.nonTerminals.IndexOf(rule.NonTerminal)] = true;
                    code += GenerateFunctionName("public", true, "void", rule.NonTerminal, "ref int poz");
                    code += "\n{\n"; nrBrace++;

                    if (_gramar.nonTerminals.Contains(rule.Rule[0])) // if it starts with nonterminal
                    {
                        code += GenerateIFName(GenerateEPSCode(rule.NonTerminal));
                        nrIf++;
                        code += "{\n";
                        nrBrace++;
                    }

                    GenerateCodeRuleBody(ref code, rule.Rule, ref nrIf, ref nrBrace);
                    if(rule.Rule.Count == 0) // if the rule si EPS
                    {
                        code += GenerateIFName(GenerateEPSCode(rule.NonTerminal));
                        nrIf++;
                        code += "{ return;";
                        nrBrace++;
                    }
                    GenerateElse(rule, ref code, ref nrBrace, ref nrIf);
                }
                else //i'm in the same function at another production rule for the same nonterminal
                {
                    GenerateCodeRuleBody(ref code, rule.Rule, ref nrIf, ref nrBrace);
                    if (rule.Rule.Count == 0)
                    {
                        code += GenerateIFName(GenerateEPSCode(rule.NonTerminal));
                        nrIf++;
                        code += "{ return; ";
                        nrBrace++;
                    }
                    GenerateElse2(rule, ref code, ref nrBrace, ref nrIf);
                }
            }
            code += "}\n}\n";
            return code;
        }
        #region Private Methods
        /// <summary>
        /// Generates else if the nonterminal is already visited
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="code"></param>
        /// <param name="nrBrace"></param>
        /// <param name="nrIf"></param>
        private void GenerateElse2(ProductionRule rule, ref string code, ref int nrBrace, ref int nrIf)
        {
            code += "}\n"; //closing the brace for last if
            nrBrace--;
            while (nrIf > 1)
            {
                code += "else { throw new Exception(\"ERROR\"); }\n";
                code += "}\n";
                nrBrace--;
                nrIf--;
            }
            if (nrIf > 0)
            {
                try
                {
                    if (_gramar.productionRules[_gramar.productionRules.IndexOf(rule) + 1].NonTerminal.Equals(rule.NonTerminal))
                    {
                        code += "else\n {\n";
                        nrIf--;
                        nrBrace++;
                    }

                    else
                    {
                        code += "else { throw new Exception(\"ERROR\"); }\n";
                        code += "}\n";
                        nrBrace--;
                        nrIf--;
                        code += "}";
                        nrBrace--; //
                    }
                }
                catch (System.ArgumentOutOfRangeException exc)
                {
                    code += "else { throw new Exception(\"ERROR\"); }\n";
                    code += "}\n";
                    nrBrace--;
                    nrIf--;
                    code += "}";
                    nrBrace--; //
                }
            }
        }
        /// <summary>
        /// Generates the else branch
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="code"></param>
        /// <param name="nrBrace"></param>
        /// <param name="nrIf"></param>
        private void GenerateElse(ProductionRule rule, ref string code, ref int nrBrace, ref int nrIf)
        {
            code += "}\n"; //closing the brace for last if
            nrBrace--;
            while (nrIf > 1)
            {
                code += "else { throw new Exception(\"ERROR\"); }\n";
                code += "}\n";
                nrBrace--;
                nrIf--;
            }
            if (nrIf > 0)
            {
                try
                {
                    if (!_gramar.productionRules[_gramar.productionRules.IndexOf(rule) + 1].NonTerminal.Equals(rule.NonTerminal))
                    {
                        code += "else { throw new Exception(\"ERROR\"); }\n";
                        code += "}\n";
                        nrBrace--;
                        nrIf--;
                    }
                    else
                    {
                        code += "else\n {\n";
                        nrIf--;
                        nrBrace++;
                    }
                }
                catch (System.ArgumentOutOfRangeException exc)
                {
                    code += "else { throw new Exception(\"ERROR\"); }\n";
                    code += "}\n";
                    nrBrace--;
                    nrIf--;
                    code += "}";
                    nrBrace--;
                }
            }
        }
        /// <summary>
        /// Generate C# body code for a production rule
        /// </summary>
        /// <param name="code"></param>
        /// <param name="rule"></param>
        /// <param name="nrIf"></param>
        /// <param name="nrBrace"></param>
        private void GenerateCodeRuleBody(ref string code, List<string> rule, ref int nrIf, ref int nrBrace)
        {
            foreach (var item in rule)
            {
                if (_gramar.terminals.Contains(item)) //if it's terminal
                {
                    code += GenerateIFName("st[poz] == \"" + item + "\"");
                    nrIf++;
                    code += "{ poz++;\n";
                    nrBrace++;
                }
                else // if it's nonterminal
                {
                    code += item + "(ref poz);\n";
                }
            }
        }
        /// <summary>
        /// Checks if only one item is visited
        /// </summary>
        /// <param name="visited">The visited array</param>
        /// <returns>True if visited and false if not.</returns>
        private bool CheckVisited(bool [] visited)
        {
            for(int i = 0; i < visited.Length; i++)
            {
                if(visited[i] == true)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Generates the matrix for LL1 analysis
        /// </summary>
        /// <returns></returns>
        private int[,] GenerateMatrix()
        {
            var matrix = new int[_gramar.nonTerminals.Count, _gramar.terminals.Count];
            for (int i = 0; i < _gramar.nonTerminals.Count; i++)
            {
                for (int j = 0; j < _gramar.terminals.Count; j++)
                {
                    matrix[i, j] = -1;
                }
            }

            foreach (var terminal in _gramar.terminals)
            {
                foreach (var productionRule in _gramar.productionRules)
                {
                    if (productionRule.DirectorSymbol.Contains(terminal))
                    {
                        matrix[_gramar.nonTerminals.IndexOf(productionRule.NonTerminal),
                               _gramar.terminals.IndexOf(terminal)] = _gramar.productionRules.IndexOf(productionRule);
                    }
                }
            }
            return matrix;
        }
        /// <summary>
        /// Returns a function antet
        /// </summary>
        /// <param name="type"> The access modifier type</param>
        /// <param name="isStatic"> Check if function is static</param>
        /// <param name="returnType"> The return type</param>
        /// <param name="name"> The function name</param>
        /// <param name="parameters"> The function's parameters</param>
        /// <returns>The C# code of function</returns>
        private string GenerateFunctionName(string type, bool isStatic, string returnType, string name, string parameters = "")
        {
            string function = "";

            function += type;
            if (isStatic)
                function += " static";
            function += " " + returnType;
            function += " " + name;
            function += "( " + parameters +")";

            return function;
        }
        /// <summary>
        /// Generate if antet
        /// </summary>
        /// <param name="boolean"></param>
        /// <returns></returns>
        private string GenerateIFName(string boolean = null)
        {
            string IF = "if(";
            IF += boolean + ")\n";
            return IF;
        }
        /// <summary>
        /// Generate the if clause for EPS production rules
        /// </summary>
        /// <param name="nonterminal"></param>
        /// <returns></returns>
        private string GenerateEPSCode(string nonterminal)
        {
            var code = "";
            var row = _gramar.nonTerminals.IndexOf(nonterminal);
            bool multiple = false;
            for(var col = 0; col < _gramar.terminals.Count; col++)
            {
                if(_matrix[row,col] != -1)
                {
                    if (multiple)
                    {
                        code += " || ";
                    }
                    multiple = true;
                    code += "st[poz] == \"" + _gramar.terminals[col] + "\""; 
                }
            }
            return code;
        }
        #endregion
        #endregion

        #region Members
        private Gramar _gramar;
        private int[,] _matrix; 
        #endregion
    }
}
