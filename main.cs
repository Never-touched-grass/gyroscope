using System;
using System.Collections.Generic;
using System.Data;
class Gs
{
    public static string EvalExp(string expr, List<string> varNames, List<object> varVals)
    {
        foreach(var name in varNames)
        {
            int index = varNames.IndexOf(name);
            if(expr.Contains(name))
            {
                expr = expr.Replace(name, varVals[index].ToString());
            }
        }
        try
        {
            var result = new DataTable().Compute(expr, null);
            return result.ToString();
        } catch
        {
            return "ERROR! Invalid expression";
        }
    }
    static void Main(string[] args)
    {
        string cmd;
        List<string> lines = new List<string>();
        List<string> varNames = new List<string>();
        List<object> varVals = new List<object>();
        int lineCount = 1;
        while (true)
        {
            Console.Write(lineCount + ".");
            cmd = Console.ReadLine();
            if (cmd == "gs--run")
            {
                break;
            }
            lines.Add(cmd);
            lineCount++;
        }
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].StartsWith("println> "))
            {
                string word = lines[i].Substring(9);
                if (word.StartsWith("'") && word.EndsWith("'"))
                {
                    string trim = word.Trim('\'');
                    Console.WriteLine(trim);
                } else
                {
                    for(int j = 0; j < varNames.Count; j++)
                    {
                        if (varNames[j] == word)
                        {
                            Console.WriteLine(varVals[j]);
                        }
                    }
                }
                
            }
            else if (lines[i].StartsWith("print> "))
            {
                string word = lines[i].Substring(7);
                if (word.StartsWith("'") && word.EndsWith("'"))
                {
                    string trim = word.Replace("'", "");
                    Console.Write(trim);
                } else
                {
                    for (int j = 0; j < varNames.Count; j++)
                    {
                        if (varNames[j] == word)
                        {
                            Console.Write(varVals[j]);
                        }
                    }
                }

                
            } else if (lines[i].StartsWith("var> "))
            {
                string line = lines[i].Substring(5);
                string[] parts = line.Split(" = ");
                if (parts.Length == 2)
                {
                    string varName = parts[0].Trim();
                    string expr = parts[1].Trim();
                    string result = EvalExp(expr, varNames, varVals);
                    int idx = varNames.IndexOf(varName);
                    if(idx != -1)
                    {
                        varVals[idx] = result;
                    } else
                    {
                        varNames.Add(varName);
                        varVals.Add(result);
                    }
                }
            } else if (lines[i].StartsWith("if> "))
            {
                string cond = lines[i].Substring(3).Trim();
                bool hasBlock = false;
                if (cond.EndsWith(" {"))
                {
                    hasBlock = true;
                    cond = cond.Substring(0, cond.Length - 1).Trim();
                }
                string[] parts = cond.Split(" ");
                if (parts.Length == 3)
                {
                    bool cm = false;
                    var left = parts[0].Trim();
                    var op = parts[1].Trim();
                    var right = parts[2].Trim();
                    switch(op)
                    {
                        case "==":
                            cm = (left == right);
                            break;
                        case "!=":
                            cm = (left != right);
                            break;
                        case "<":
                            if(int.TryParse(left, out int leftInt) && int.TryParse(right, out int rightInt))
                            {
                                cm = (leftInt < rightInt);
                                break;
                            } else
                            {
                                Console.WriteLine("ERROR: Can only compare integers with < operator.");
                                break;
                            }
                        case ">":
                            if (int.TryParse(left, out int leftInt2) && int.TryParse(right, out int rightInt2))
                            {
                                cm = (leftInt2 > rightInt2);
                                break;
                            }
                            else
                            {
                                Console.WriteLine("ERROR: Can only compare integers with > operator.");
                                break;
                            }
                        case "<=":
                            if (int.TryParse(left, out int leftInt3) && int.TryParse(right, out int rightInt3))
                            {
                                cm = (leftInt3 <= rightInt3);
                                break;
                            }
                            else
                            {
                                Console.WriteLine("ERROR: Can only compare integers with <= operator.");
                                break;
                            }
                        case ">=":
                            if (int.TryParse(left, out int leftInt4) && int.TryParse(right, out int rightInt4))
                            {
                                cm = (leftInt4 >= rightInt4);
                                break;
                            }
                            else
                            {
                                Console.WriteLine("ERROR: Can only compare integers with >= operator.");
                                break;
                            }
                    }
                    if (!cm)
                    {
                        while (i < lines.Count && lines[i] != "}")
                        {
                            i++;
                        }
                        continue;
                    }
                }
            } else if (lines[i].StartsWith("read> "))
            {
                string varName = lines[i].Substring(6).Trim();
                for(int j = 0; j < varNames.Count; j++)
                {
                    if (varNames[j] == varName)
                    {
                        varVals[j] = Console.ReadLine();
                    }
                }
            } else if (lines[i].StartsWith("while> "))
            {
                string cond = lines[i].Substring(7).Trim();
                bool hasBlock = false;
                if (cond.EndsWith(" {"))
                {
                    hasBlock = true;
                    cond = cond.Substring(0, cond.Length - 1).Trim();
                }
                if (hasBlock)
                {
                    string[] parts = cond.Split(" ");
                    if (parts.Length == 3)
                    {
                        string varname = parts[0].Trim();
                        string op = parts[1].Trim();
                        string value = parts[2].Trim();
                        int start = i + 1;
                        int end = start;
                        while(end < lines.Count && lines[end] != "}")
                        {
                            end++;
                        }
                        List<string> lblock = lines.GetRange(start, end - start);
                        var vIndex = varNames.IndexOf(varname);
                        if (vIndex != -1 && int.TryParse(varVals[vIndex].ToString(), out int rVal))
                        {
                            while(true)
                            {
                                int lVal;
                                int riVal;
                                string leftEval = EvalExp(varVals[vIndex].ToString(), varNames, varVals);
                                string rightEval = EvalExp(value, varNames, varVals);
                                if (!int.TryParse(leftEval, out lVal) || !int.TryParse(rightEval, out riVal)) {
                                    break;
                                }
                                bool cm = false;
                                switch(op)
                                {
                                    case "<":
                                        cm = (lVal < riVal);
                                        break;
                                    case ">":
                                        cm = (lVal > riVal);
                                        break;
                                    case "<=":
                                        cm = (lVal <= riVal);
                                        break;
                                    case ">=":
                                        cm = (lVal >= riVal);
                                        break;
                                    case "!=":
                                        cm = (lVal != riVal);
                                        break;
                                }
                                if (!cm)
                                {
                                    break;
                                }
                                for (int li = 0; li < lblock.Count; li++)
                                {
                                    string line = lblock[li];
                                    if (line.StartsWith("println> "))
                                    {
                                        string word = line.Substring(9);
                                        if (word.StartsWith("'") && word.EndsWith("'"))
                                        {
                                            Console.WriteLine(word.Trim('\''));
                                        }
                                        else
                                        {
                                            int idx = varNames.IndexOf(word);
                                            if (idx != -1)
                                                Console.WriteLine(varVals[idx]);
                                        }
                                    }
                                    else if (line.StartsWith("print> "))
                                    {
                                        string word = line.Substring(7);
                                        if (word.StartsWith("'") && word.EndsWith("'"))
                                        {
                                            Console.Write(word.Trim('\''));
                                        }
                                        else
                                        {
                                            int idx = varNames.IndexOf(word);
                                            if (idx != -1)
                                                Console.Write(varVals[idx]);
                                        }
                                    }
                                    else if (line.StartsWith("var> "))
                                    {
                                        string[] prs = line.Substring(5).Split(" = ");
                                        if (prs.Length == 2)
                                        {
                                            string name = prs[0].Trim();
                                            string val = prs[1].Trim();
                                            string result = EvalExp(val, varNames, varVals);
                                            int idx = varNames.IndexOf(name);
                                            if (idx != -1)
                                                varVals[idx] =  result;
                                            else
                                            {
                                                varNames.Add(name);
                                                varVals.Add(result);
                                            }
                                        }
                                    }
                                }


                            }
                        }
                        i = end;
                    }
                }
            }
        }
    }
}
