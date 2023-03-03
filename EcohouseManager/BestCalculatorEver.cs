using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Text.RegularExpressions;
using System.Reflection;

namespace EcohouseManager {

    /// <summary>
    /// Калькулятор с возможностью подстановки переменных.
    /// </summary>

    static class BestCalculatorEver
    {
		// Перечисление типов поддерживаемых команд.
		
		private enum CmdType
		{
			CMath,
			CCompare
		}		

		// Структура, содержащая информацию о поддерживаемой команды.
		
		private struct CommandInfo
		{
			// Тип команды.
			
			public CmdType CType { get; }
			
			public string CRealName { get; }

            public CommandInfo(CmdType cType, string cRealName)
            {
                CType = cType;
                CRealName = cRealName;
            }
		}

        // Словарь поддерживаемых команд.

        private static Dictionary<string, CommandInfo> Commands =
        new Dictionary<string, CommandInfo>
        {
            { "abs", new CommandInfo(cType : CmdType.CMath, cRealName : "Abs") },
            { "sqrt", new CommandInfo(cType : CmdType.CMath, cRealName : "Sqrt") },
            { "floor", new CommandInfo(cType : CmdType.CMath, cRealName : "Floor") },
            { "ceiling", new CommandInfo(cType : CmdType.CMath, cRealName : "Ceiling") },
            { "sin", new CommandInfo(cType : CmdType.CMath, cRealName : "Sin") },
            { "cos", new CommandInfo(cType : CmdType.CMath, cRealName : "Cos") },
            { "tan", new CommandInfo(cType : CmdType.CMath, cRealName : "Tan") },
            { "min", new CommandInfo(cType : CmdType.CCompare, cRealName : "Min") },
            { "max", new CommandInfo(cType : CmdType.CCompare, cRealName : "Max") }
        };

        // Математические константы.

        private static Dictionary<string, decimal> MathConstants =
            new Dictionary<string, decimal>
            {
                { "pi", (decimal)Math.PI },
                { "e", (decimal)Math.E }
            };

        // Глобальные константы проекта.

        private static ObservableDictionary<string, decimal> ProjVariables;

        // Локальные переменные проекта.

        private static Dictionary<string, decimal> ProjConstants;

        // Документ, из которого берутся переменные.

        private static XmlDocument xmlVars;

        // Выборка параметров команды.

        private static Regex ExtractCommandParams;

        // Разборка выражения на составляющие.

        private static Regex DisassembleExpression;

        // Нахождение команды.

        private static Regex FindCommand;

        // Статический конструктор.

        static BestCalculatorEver()
        {
            string patternECP = @"(?<params>[^\(;\)]+;?)+";

            ExtractCommandParams = new Regex(patternECP, RegexOptions.Compiled);

            string patternDE = @"(" +
                               @"(?<digitals>(\B\-)?\d+(\,\d+(E[\+-]\d+)?)?)|" +
                               @"(?<operations>[\+-\/%\^\*])|" +
                               @")" +
                               @"{1,}";

            DisassembleExpression = new Regex(patternDE, RegexOptions.Compiled);

            string patternFC = @"\w+$";

            FindCommand = new Regex(patternFC, RegexOptions.Compiled |
                                               RegexOptions.RightToLeft);

            xmlVars = new XmlDocument();
            string path = System.IO.Path.GetDirectoryName
                (System.Reflection.Assembly.GetExecutingAssembly().Location) +
                @"\Variables.xml";
            xmlVars.Load(path);

            ProjConstants = new Dictionary<string, decimal>();
            SetProjContext();
            ProjVariables = null;
        }

        // Установить контекст переменных.

        private static void SetProjContext()
        {
            XmlNodeList consts = xmlVars.DocumentElement.SelectNodes("const");

            ProjConstants.Clear();

            foreach (XmlNode xmlConst in consts)
            {
                string constName = xmlConst.Attributes.GetNamedItem("name").Value;

                if (ProjConstants.Keys
                    .Contains(constName))
                    throw new Exception("Переменная уже существует: " + constName);

                decimal cnst;
                if (!decimal.TryParse(xmlConst.InnerText, out cnst))
                    throw new Exception("Переменная не является числом: " + constName);

                ProjConstants.Add(constName, cnst);
            }
        }

        // Обновить глобальные константы проекта.

        public static void UpdateProjectConstants()
        {
            SetProjContext();
        }

        // Обновить локальные переменные.

        public static void UpdateProjectVariables
            (ObservableDictionary<string, decimal> vars)
        {
            ProjVariables = vars;
        }

        public static string[] GetSupportedMathConstants()
        {
            return MathConstants.Keys.ToArray();
        }

        // Запуск калькулятора.

        public static decimal Run(string expr)
		{
            ReplaceVars();

            StringBuilder exprBuilder = new StringBuilder(expr);

            for (int i = 0; i < exprBuilder.Length; )
                if (exprBuilder[i] == ' ')
                    exprBuilder.Remove(i, 1);
                else i++;

            expr = exprBuilder.ToString();

            double calc = Calculate(expr);
            decimal result;

            /*
             * Если в итоге вышла ерунда, то нужно что-то предпринять.
             */

            if (calc is double.NaN)
            {
                result = -39909;
            }
            else
                result = Convert.ToDecimal(calc);

            return result;

            // Замещение имён переменных их значениями.

            void ReplaceVars()
            {
                Regex findVars = new Regex(@"\b[A-Za-zА-Яа-я_][_\w]*\(?",
                                           RegexOptions.Compiled);

                foreach (Match varMatch in findVars.Matches(expr))
                {
                    string varName = varMatch.Value;
                    // Нормально имена переменных не ищутся,
                    // приходится пропускать имена команд.
                    if (varName.Last() == '(')
                        continue;

                    Regex replaceVar = new Regex(varName);

                    decimal varValue = 0;
                    try
                    {
                        if (!MathConstants.TryGetValue(varName, out varValue))
                            if (!ProjVariables.TryGetValue(varName, out varValue))
                                if (!ProjConstants.TryGetValue(varName, out varValue))
                                {
                                    throw new Exception
                                          ("Переменная не найдена: " + varName);
                                }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                    finally
                    {
                        expr = replaceVar
                                  .Replace(expr, varValue.ToString());
                    }
                }
            }

        }

        // Информация о типе выражения.
        // Main в отсортированном по скобочной глубине списке
        // должен стоять выше в любом случае;
        // SubExpr - выражение, состоящее из чисел и операций;
        // Command - выражение команды с её именем, скобками и параметрами.

        private enum ExprType
        {
            MainExpr = 2,
            SubExpr = 1,
            Command = 0
        }

        // Класс с информацией о выражении.

        private class ExprInfo : IComparable<ExprInfo>
        {
            // Индекс начала выражения относительно
            // главного выражения.

            public int StartIndex { get; }

            // Индекс конца выражения относительно
            // главного выражения.

            public int EndIndex { get; set; }

            // Глубина скобочного погружения.

            public int Depth { get; }

            // Тип выражения.

            public ExprType EType { get; }


            // Выражения в составе данного.
            // Позже части данного выражения заменяются их значениями.

            public List<ExprInfo> ContainedExpressions { get; }

            // Начальный вид выражения.
            // как оно было записано в главном.

            public string StartStringValue { get; set; }

            // Промежуточный вид выражения
            // на этапе замены его частей их значениями.

            public StringBuilder IntermediateStringValue { get; set; }

            // Итоговый вид выражения.

            public string ResultValue { get; set; }

            // Конструктор.

            public ExprInfo(int start, int depth, ExprType eType)
            {
                StartIndex = start;
                EndIndex = -1;
                Depth = depth;
                EType = eType;
                ContainedExpressions = new List<ExprInfo>(0);
            }

            // Сравнение выражений по их скобочной глубине.
            // Применяется при сортировке выражений.

            public int CompareTo(ExprInfo eInfo2)
            {
                if (eInfo2 == null)
                    return 1;

                int byDepth = Depth.CompareTo(eInfo2.Depth);

                if (byDepth == 0)
                    return EType.CompareTo(eInfo2.EType);
                else
                    return byDepth;
            }
        }

        // Отдельный класс для выражений команд.
        // Сомнительная необходимость, но пущай будет.

        private class CmdExprInfo : ExprInfo
        {
            public string Name { get; }

            public CmdExprInfo(int start, int depth, ExprType eType, string name) 
                                : base (start, depth, eType)
            {
                Name = name;
            }
        }

        // Вычислить выражение с заменой переменных и поиском подвыражений.

        private static double Calculate(string expr)
        {
            StringBuilder exprBuilder = new StringBuilder(expr);

            int curDepth = 0, maxDepth = 0;
            Stack<ExprInfo> expressions = new Stack<ExprInfo>();
            expressions.Push(new ExprInfo(0, 0, ExprType.MainExpr) { EndIndex = -1 });
            List<ExprInfo> expressionsSortedByDepth = new List<ExprInfo>()
                                                      { expressions.Peek() };
            expressionsSortedByDepth[0].StartStringValue = expr;

            for (int i = 0; i < exprBuilder.Length; i++)
            {
                if (exprBuilder[i] == '(')
                {
                    Match cmdMatch = FindCommand.Match(expr.Substring(0, i));

                    ExprInfo eInfo;

                    if (!string.IsNullOrEmpty(cmdMatch.Value))
                    {
                        string cmdName = cmdMatch.Value;
                        eInfo = new CmdExprInfo(i - cmdName.Length, curDepth, 
                                                ExprType.Command, cmdName);
                        expressions.Push(eInfo);
                    }
                    else
                    {
                        curDepth--;
                        if (curDepth < maxDepth)
                            maxDepth = curDepth;

                        eInfo = new ExprInfo(i, curDepth, ExprType.SubExpr);
                        expressions.Push(eInfo);
                    }

                    ExprInfo parent = expressions.Skip(1)
                                      .SkipWhile(e => e.EndIndex != -1)
                                      .First();
                    parent.ContainedExpressions.Add(eInfo);

                    continue;
                }

                if (exprBuilder[i] == ')')
                {
                    ExprInfo eInfo = expressions.Pop();

                    if (eInfo.EType == ExprType.SubExpr)
                        curDepth++;

                    eInfo.EndIndex = i;
                    eInfo.StartStringValue = expr.Substring(eInfo.StartIndex,
                                        eInfo.EndIndex - eInfo.StartIndex + 1);
                    expressionsSortedByDepth.Add(eInfo);
                }
            }

            expressionsSortedByDepth.Sort();

            foreach (ExprInfo eInfo in expressionsSortedByDepth)
            {
                eInfo.IntermediateStringValue = 
                    new StringBuilder(eInfo.StartStringValue);
                if (eInfo.EType == ExprType.SubExpr)
                {
                    eInfo.IntermediateStringValue.Remove(0, 1);
                    eInfo.IntermediateStringValue
                        .Remove(eInfo.IntermediateStringValue.Length - 1, 1);
                }

                // Расчёт простейших выражений.

                if (eInfo.ContainedExpressions.Count == 0)
                    CalculateExpression(eInfo);

                // Расчёт выражений посложнее.

                else
                {
                    foreach (ExprInfo child in eInfo.ContainedExpressions)
                    {
                        ReplaceExpression(eInfo.IntermediateStringValue,
                                          child.StartStringValue,
                                          child.ResultValue);
                    }

                    CalculateExpression(eInfo);
                }
            }

            double result;

            try {  result = Convert.ToDouble(expressionsSortedByDepth.Last().ResultValue); }
            catch { result = double.NaN; }

            return result; 



            void ReplaceExpression(StringBuilder targetExpr, 
                                     string replacedExpr, 
                                     string replacedExprValue)
            {
                targetExpr.Replace(replacedExpr, replacedExprValue);
            }
			
			// Вызов команды.
			
			double CallCommand(CommandInfo ci, double[] cParams)
			{
				Type cmdType = null;
                object[] cmdParams = new object[1];
                Type[] signature = null;

                switch (ci.CType)
                {
                    case CmdType.CMath:
                        {
                            cmdType = typeof(Math);
                            cmdParams[0] = cParams[0];
                            signature = new Type[] { typeof(double) };
                            break;
                        }
                    case CmdType.CCompare:
                        {
                            cmdType = typeof(Enumerable);
                            cmdParams[0] = cParams;
                            signature = new Type[] { typeof(double[]) };
                            break;
                        }
                }
                
                MethodInfo cmdMethod = cmdType.GetMethod(ci.CRealName, signature);

                double outParam = (double)cmdMethod.Invoke(null, cmdParams);

                return outParam;
			}

            // Вычисление простого значения.

            string CalculateSimpleExpression(string simpleExpr)
			{
                Match deMatch = DisassembleExpression.Match(simpleExpr);

                StringBuilder simpleExprBuilder = new StringBuilder(simpleExpr);

                ProcessOperations(@"\^");
                ProcessOperations(@"\*\/");
                ProcessOperations(@"\+-");

                return simpleExprBuilder.ToString();

                void ProcessOperations(string operations)
                {
                    string patternOperations =
                        @"(?<left>(\B\-)?\d+(\,\d+(E[\+-]\d+)?)?)" +
                        @"(?<operation>[" + operations + "])" +
                        @"(?<right>(\B\-)?\d+(\,\d+(E[\+-]\d+)?)?)";

                    Regex atomicExprRg = new Regex(patternOperations,
                                                 RegexOptions.Compiled);

                    double leftOperand = 0, rightOperand = 0, res = 0;
                    string operation = "";

                    Match atomicExprMatch;

                    while ((atomicExprMatch = atomicExprRg
                           .Match(simpleExprBuilder.ToString()))
                           .Value != string.Empty)
                    {
                        string atomicExpr = atomicExprMatch.Value;

                        leftOperand = Convert.ToDouble
                            (atomicExprMatch.Groups["left"].Value);
                        operation = atomicExprMatch.Groups["operation"].Value;
                        rightOperand = Convert.ToDouble
                            (atomicExprMatch.Groups["right"].Value);

                        switch (operation)
                        {
                            case ("+"): 
                                { res = leftOperand + rightOperand; break; }
                            case ("-"): 
                                { res = leftOperand - rightOperand; break; }
                            case ("*"): 
                                { res = leftOperand * rightOperand; break; }
                            case ("/"):
                                {
                                    try { res = leftOperand / rightOperand; }
                                    catch (DivideByZeroException ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                    break;
                                }
                            case ("^"):
                                { res = Math.Pow(leftOperand, rightOperand); break; }
                        }

                        ReplaceExpression(simpleExprBuilder,
                                          atomicExpr,
                                          res.ToString());
                    }

                    return;
                }
			}

            void CalculateExpression(ExprInfo eInfo)
            {
                switch (eInfo.EType)
                {
                    case ExprType.MainExpr: case ExprType.SubExpr:
                        {
                            eInfo.ResultValue = CalculateSimpleExpression
                                (eInfo.IntermediateStringValue.ToString());
                            break;
                        }
                    case ExprType.Command:
                        {
                            string cmdExpr = eInfo.IntermediateStringValue.ToString(),
                                   cmdName = ((CmdExprInfo)eInfo).Name,
                                   cmdParamsStr = cmdExpr.Substring(cmdName.Length + 1);
                            cmdParamsStr = cmdParamsStr.Remove(cmdParamsStr.Length - 1);

                            List<double> paramsValues = new List<double>();

                            // Добавление параметров.

                            foreach (Capture cmdParamCapture in ExtractCommandParams
                                                                .Match(cmdParamsStr)
                                                                .Groups["params"]
                                                                .Captures)
                            {
                                string param = CalculateSimpleExpression
                                                (cmdParamCapture.Value.Trim(';'));
                                paramsValues.Add(Convert.ToDouble(param));
                            }

                            CommandInfo cInfo = Commands[cmdName];

                            double cmdResult = CallCommand
                                               (cInfo, paramsValues.ToArray());

                            eInfo.ResultValue = cmdResult.ToString();

                            break;
                        }
                }
            }
        }
    }
}
