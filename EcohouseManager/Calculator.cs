using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace EcohouseManager {
    static class Calculator {

        static int i = 0;

        public static double StringCalculator(string expression, XmlDocument xDoc) {
            if (expression == "")
                return 0;
            return Calculate(ExpressionReader(expression, xDoc));
        }

        public static string ExpressionReader(string expression, XmlDocument xDoc) {
            Dictionary<string, string> vars = new Dictionary<string, string>();

            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Variables.xml";

            XmlDocument variables = new XmlDocument();
            variables.Load(path);

            foreach (XmlNode child in variables.SelectSingleNode("constants").ChildNodes) 
                vars.Add(child.Attributes.GetNamedItem("name").Value, child.InnerText);
            foreach (XmlNode child in xDoc.SelectSingleNode("root/variables").ChildNodes) 
                vars.Add(child.Name, child.InnerText);

            foreach (string s in vars.Keys) {
                expression = expression.Replace(s, vars[s]);
            }

            int b = 0;

            for(int i = 0; i < expression.Length - 1; i++) {
                if(expression[i] == '*') {
                    for (int k = -1; k < 2; k += 2) {
                        char c = k < 0 ? '(' : ')', cInv = k > 0 ? '(' : ')';
                        int n = k < 0 ? 0 : (expression.Length - 1);
                        for (int j = i + k; j != n + k; j += k) {
                            void adb() {
                                expression = expression.Insert(j + k + (k > 0 ? 0 : 1), c.ToString());
                                if (c == '(')
                                    i++;
                            }
                            if (expression[j] == cInv)
                                b++;
                            if (expression[j] == c) {
                                b--;
                                if (b == 0) {
                                    adb();
                                    break;
                                }
                            }
                            if (j == n) {
                                adb();
                                break;
                            } 
                            else
                            if (!isDigit(expression[j + k]) && b == 0) {
                                adb();
                                break;
                            }
                        }
                    }
                }
            }
            expression += "=";

            return expression;
        }

        private static double Calculate(string expression) {

            if (expression == null)
                return 0;

            double result = 0;

            string num = "0", lastAct = "+";

            while(i < expression.Length) {
                if (isDigit(expression[i]))
                    num += expression[i];
                else
                if (expression[i] == '(' || expression[i] == '{' || expression[i] == '[') {
                    i++;
                    num = Convert.ToString(Calculate(expression));
                    double n = Convert.ToDouble(num);
                    if (expression[i] == '}') {
                        num = Convert.ToString(Math.Round(n));
                    }
                    if (expression[i] == ']')
                        num = Convert.ToString(Math.Sqrt(n));
                }
                else
                if (expression[i] == ')' || expression[i] == '}' || expression[i] == ']') {

                    double n = Convert.ToDouble(num);

                    result = Action(result, n, lastAct);
                    num = "0";
                    lastAct = Convert.ToString(expression[i]);

                    return result;
                }
                else {
                    double n = Convert.ToDouble(num);

                    result = Action(result, n, lastAct);
                    num = "0";
                    lastAct = Convert.ToString(expression[i]);
                }

                i++;
            }

            i = 0;
            return result;
        }

        private static double Action(double result, double n, string lastAct) {
            switch (lastAct) {
                case ("+"): { result += n; } break;
                case ("-"): { result -= n; } break;
                case ("*"): { result *= n; } break;
                case ("/"): { result /= n; } break;
                case ("^"): { result = Math.Pow(result, n); } break;
            }
            return result;
        }

        private static bool isDigit(char ch) {
            if (ch >= '0' && ch <= '9' || ch == ',')
                return true;
            else
                return false;
        }
    }
}
