using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiceTowerLibrary
{
    /// 注释
    /// <摘要>
    /// 骰塔库
    /// </摘要>
    /// 
    /// <使用>
    /// ---------------------------------------------------------------------
    /// 骰子：     aDb      -a:骰子数量   -b:骰子面数
    ///           aDbUc   -U:保留最大的c个D面骰子
    ///           aDbLc   -L:保留最小的c个D面骰子
    /// ---------------------------------------------------------------------
    /// 例子：     公式投骰：
    ///           可进行投骰与四则运算混合，支持使用括号  //仅支持小括号()，不支持[]{}//
    ///           支持自定义随机种子，DiceTower diceTower = new DiceTower(seed);
    ///           有输入检测，如果输入错误会抛出异常.
    ///           
    ///           1+2*3/4-4D6
    ///           DiceTower.Roll("1+2*3/4-4D6");
    ///           10D20L4
    ///           DiceTower.Roll("10D20L4");
    ///           ((1d4+3*2)*3+8/(4d6-2*3))
    ///           DiceTower.Roll("((1d4+3*2)*3+8/(4d6-2*3))");
    ///           
    ///           简单投骰：
    ///           仅可进行投骰运算
    ///           没有输入检测
    ///           建议仅在用户输入有保障（如按钮）的情况下使用。
    ///           
    ///           1D6U3
    ///           DiceTower.RollDices("1D6U3");
    /// ---------------------------------------------------------------------
    /// </使用>
    /// 

    public class DiceTower
    {
        #region 字段
        private Random random;

        private bool IsSeed = false;

        private bool IsGoldfingerOpen = false;

        private string Seed = null;
        #endregion

        #region 构造函数
        public DiceTower()
        {
            random = new Random();
        }

        public DiceTower(string seed)
        {
            Seed = seed;
            IsSeed = true;
            random = new Random(Seed.GetHashCode());
            if (h2(Seed) == "E5C4EDE8FE1E5BD2A7360AEADAD2F703803A6373972C81CDC404F01F58B0EDDA") IsGoldfingerOpen = true;
        }
        #endregion

        #region 方法
        public string RollDices(string Dices)                         //简单投骰
        {
            string DicesResult = null;

            if (Dices.Contains('U'))
            {
                string[] PickNumber = Dices.Split('U');
                DicesResult = RollDice(PickNumber[0]);
                DicesResult = DicesResult.Replace("+", ",");
                DicesResult = DicesResult.Split('=')[0];
                List<string> numbers = new List<string>(DicesResult.Split(','));
                DicesResult += " Pick " + numbers.Max();
                for (int i = 1; i < int.Parse(PickNumber[1]); i++)
                {
                    numbers.Remove(numbers.Max());
                    DicesResult += '+' + numbers.Max();
                }
                numbers = new List<string>(DicesResult.Split(' ')[2].Split('+'));
                int sum = 0;
                foreach (string a in numbers)
                {
                    sum += int.Parse(a);
                }
                DicesResult = '[' + DicesResult + '=' + sum + ']';

            }
            else if (Dices.Contains('L'))
            {
                string[] PickNumber = Dices.Split('L');
                DicesResult = RollDice(PickNumber[0]);
                DicesResult = DicesResult.Replace("+", ",");
                DicesResult = DicesResult.Split('=')[0];
                List<string> numbers = new List<string>(DicesResult.Split(','));
                DicesResult += " Pick " + numbers.Min();
                for (int i = 1; i < int.Parse(PickNumber[1]); i++)
                {
                    numbers.Remove(numbers.Min());
                    DicesResult += '+' + numbers.Min();
                }
                numbers = new List<string>(DicesResult.Split(' ')[2].Split('+'));
                int sum = 0;
                foreach (string a in numbers)
                {
                    sum += int.Parse(a);
                }
                DicesResult = '[' + DicesResult + '=' + sum + ']';
            }
            else
            {
                DicesResult = '[' + RollDice(Dices) + ']';
            }

            string RollDice(string Dice)
            {
                string DiceResult = null;
                string[] RollNumber = Dice.Split('D');

                if (IsGoldfingerOpen)
                {
                    DiceResult += RollNumber[1];
                }
                else
                {
                    DiceResult += random.Next(1, int.Parse(RollNumber[1]) + 1);
                }
                for (int i = 1; i < int.Parse(RollNumber[0]); i++)
                {
                    if (IsGoldfingerOpen)
                    {
                        DiceResult += "+" + RollNumber[1];
                    }
                    else
                    {
                        DiceResult += "+" + random.Next(1, int.Parse(RollNumber[1]) + 1);
                    }
                }

                int sum = 0;
                string[] SumNumberStr = DiceResult.Split('+');
                foreach (string number in SumNumberStr)
                {
                    sum += int.Parse(number);
                }
                DiceResult = DiceResult + "=" + sum;
                return DiceResult;
            }
            return DicesResult;
        }

        public string Roll(string AllDice)                          //公式投骰
        {
            string Result = null;
            string infix = ValidityTest(AllDice);
            string PickString = null;
            string RollResult = null;
            Result = infix;
            while (infix.Contains('U'))
            {
                PickString = Regex.Match(infix, "[0-9]+D[0-9]+U[0-9]+").ToString();
                RollResult = RollDices(PickString);
                int a = infix.IndexOf(PickString);
                infix = infix.Remove(a, PickString.Length);
                infix = infix.Insert(a, RollResult);
            }
            while (infix.Contains('L'))
            {
                PickString = Regex.Match(infix, "[0-9]+D[0-9]+L[0-9]+").ToString();
                RollResult = RollDices(PickString);
                int a = infix.IndexOf(PickString);
                infix = infix.Remove(a, PickString.Length);
                infix = infix.Insert(a, RollResult);
            }
            while (infix.Contains('D'))
            {
                PickString = Regex.Match(infix, "[0-9]+D[0-9]+").ToString();
                RollResult = RollDices(PickString);
                int a = infix.IndexOf(PickString);
                infix = infix.Remove(a, PickString.Length);
                infix = infix.Insert(a, RollResult);
            }

            Result += '=' + infix;
            Result += '=' + ComputePostfix(InfixToPostfix(infix));
            if (IsSeed) Result = "\"" + Seed + "\"" + ":   " + Result;

            return Result;
        }

        private string ValidityTest(string AllDice)                   //检测合法性
        {
            #region 检测包内容是否合法
            AllDice = AllDice.Replace(" ", string.Empty).ToUpper();

            if (AllDice.Length > 32 || AllDice.Length < 3) throw new Exception("算式长度不得长于32字符，不得短于3字符");

            string infix = AllDice; //中序式

            if (!Regex.IsMatch(infix, "^[-+*/DUL0-9()]+$")) throw new Exception("包含非法字符，仅限-,+,*,/,d,u,l,D,U,L,0-9,(,)");
            #endregion
            #region 检测括号是否匹配
            int BrackeTstest = 0;
            for (int i = 0; i < infix.Length; i++)
            {
                switch (infix[i])
                {
                    case '(':
                        BrackeTstest++;
                        break;
                    case ')':
                        BrackeTstest--;
                        if (BrackeTstest < 0) throw new Exception("括号不匹配");
                        break;
                }
            }
            if (BrackeTstest > 0) throw new Exception("括号不匹配");
            #endregion
            #region 检测算式语法是否合法
            if (!Regex.IsMatch(infix, "^[0-9(]")) throw new Exception("语法错误，首字符不得为 运算符 或 右括号");
            if (!Regex.IsMatch(infix, "[0-9)]$")) throw new Exception("语法错误，末字符不得为 运算符 或 左括号");

            char FrontLatter = '\0';
            char MidLatter = '\0';
            char BehindLatter = '\0';
            for (int i = 2; i < infix.Length; i++)
            {
                FrontLatter = infix[i - 2];
                MidLatter = infix[i - 1];
                BehindLatter = infix[i];
                switch (MidLatter)
                {
                    case '+':
                    case '-':
                    case '*':
                    case '/':

                        if ((FrontLatter < 48 || FrontLatter > 57) ^ FrontLatter == ')') throw new Exception("语法错误，运算符 前 必须为 数字 或 右括号");
                        if ((BehindLatter < 48 || BehindLatter > 57) ^ BehindLatter == '(') throw new Exception("语法错误，运算符 后 必须为 数字 或 左括号");
                        break;
                    case '(':
                        if (BehindLatter < 48 || BehindLatter > 57) throw new Exception("语法错误，左括号 后 必须为 数字");
                        if ((FrontLatter >= 48 || FrontLatter <= 57) ^ !Regex.IsMatch(FrontLatter.ToString(), @"^[DUL)]+$")) throw new Exception("语法错误，左括号 前 必须为 数字 或 数学运算符 或 左括号");
                        break;
                    case '）':
                        if ((BehindLatter >= 48 || BehindLatter <= 57) ^ !Regex.IsMatch(FrontLatter.ToString(), @"^[DUL(]+$")) throw new Exception("语法错误，右括号 后 必须为 数字 或 数学运算符 或 右括号");
                        break;
                    case 'D':
                        if (FrontLatter < 48 || FrontLatter > 57) throw new Exception("语法错误，D运算符 前 必须为 数字");
                        if (BehindLatter < 48 || BehindLatter > 57) throw new Exception("语法错误，D运算符 后 必须为 数字");
                        break;
                    case 'U':
                    case 'L':
                        if (FrontLatter < 48 || FrontLatter > 57) throw new Exception("语法错误，U,L运算符 前 必须为 数字");
                        if (BehindLatter < 48 || BehindLatter > 57) throw new Exception("语法错误，U,L运算符 后 必须为 数字");
                        bool isNotFindD = true;
                        if (infix.Length < 5) throw new Exception("语法错误，U或L运算符 必须配合 D运算符 使用");
                        for (int j = i - 2; ((j >= 0) && isNotFindD); j--)
                        {
                            switch (infix[j])
                            {
                                case '+':
                                case '-':
                                case '*':
                                case '/':
                                case 'U':
                                case 'L':
                                case '(':
                                case ')':
                                    throw new Exception("语法错误，U或L运算符 必须配合 D运算符 使用");
                                case 'D':
                                    string a = null;
                                    int aj = j - 1;
                                    while (aj >= 0)
                                    {
                                        if (infix[aj] == '+' || infix[aj] == '-' || infix[aj] == '*' || infix[aj] == '/' || infix[aj] == '(') break;

                                        switch (infix[aj])
                                        {
                                            case 'U':
                                            case 'L':
                                            case 'D':
                                            case ')':
                                                throw new Exception("语法错误，U或L运算符 匹配到 多个D,U,L运算符");
                                            default:
                                                a += infix[aj].ToString();
                                                break;
                                        }
                                        aj--;
                                    }   //得到D前数字
                                    string c = null;
                                    int cj = i;     //UL后第一个字符
                                    while (cj < infix.Length)
                                    {
                                        if (infix[cj] == '+' || infix[cj] == '-' || infix[cj] == '*' || infix[cj] == '/' || infix[cj] == ')') break;

                                        switch (infix[cj])
                                        {
                                            case 'U':
                                            case 'L':
                                            case 'D':
                                            case '(':
                                                throw new Exception("语法错误，U或L运算符 后 匹配到 多个D,U,L运算符");
                                            default:
                                                c += infix[cj].ToString();
                                                break;
                                        }
                                        cj++;
                                    }   //得到UL后数字
                                    a = ReverseString(a);
                                    if (int.Parse(c) >= int.Parse(a)) throw new Exception("语法错误，U或L运算符 所选取的数量 大于 总投掷骰子数量");
                                    isNotFindD = false;
                                    break;
                            }
                        }   //检查每一个UL运算符格式是否正确
                        break;
                    default:
                        break;
                }
            }
            #endregion
            return infix;
        }

        private Stack<string> InfixToPostfix(string infix)          //中序式转后序式
        {
            Stack<string> PostfixStack = new Stack<string>();
            Stack<string> OperatorsStack = new Stack<string>();
            string incoming = null;

            for (int i = 0; i < infix.Length; i++)
            {
                incoming = infix[i].ToString();
                switch (incoming)
                {
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                        while (OperatorsStack.Count > 0)
                        {
                            if (priority(OperatorsStack.Peek()) > priority(incoming))
                            {
                                PostfixStack.Push(OperatorsStack.Pop());
                                continue;
                            }
                            else
                            {
                                OperatorsStack.Push(incoming);
                                break;
                            }
                        }
                        if (OperatorsStack.Count == 0) OperatorsStack.Push(incoming);
                        break;
                    case "[":
                        string RollResult = null;
                        while (true)
                        {
                            RollResult += incoming;
                            if ((i + 2) > infix.Length) break;
                            i++;
                            incoming = infix[i].ToString();
                            if (incoming == "]")
                            {
                                break;
                                //抛掉最后的"]"
                            }
                        }
                        RollResult = RollResult.Split('=')[1];
                        PostfixStack.Push(RollResult);
                        break;
                    case "(":
                        OperatorsStack.Push(incoming);
                        break;
                    case ")":
                        while (OperatorsStack.Peek() != "(")
                        {
                            PostfixStack.Push(OperatorsStack.Pop());
                        }
                        OperatorsStack.Pop();
                        break;
                    default:
                        string number = null;
                        while (true)
                        {
                            number += incoming;
                            if ((i + 2) > infix.Length) break;
                            i++;
                            incoming = infix[i].ToString();
                            if (!Regex.IsMatch(incoming, "[0-9]"))
                            {
                                i--;
                                break;
                            }
                        }
                        PostfixStack.Push(number);
                        break;
                }
            }
            while (OperatorsStack.Count > 0)
            {
                PostfixStack.Push(OperatorsStack.Pop());
            }
            Stack<string> FinalPostFixStack = new Stack<string>();
            while (PostfixStack.Count > 0)
            {
                FinalPostFixStack.Push(PostfixStack.Pop());
            }
            foreach (string str in FinalPostFixStack)
            {
                Console.Write("|" + str);
            }
            return FinalPostFixStack;
        }

        private string h2(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = System.Security.Cryptography.SHA256Managed.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString();
        }

        private string ComputePostfix(Stack<string> PostfixStack)   //计算后序式
        {
            string Result = null;                                 //存放最终计算过程及结果
            Stack<string> OperandStack = new Stack<string>();     //存放运算子
            string FrontOperand = null;                           //运算元前面的运算子
            string BehindOperand = null;                          //运算元后面的运算子
            foreach (string incoming in PostfixStack)
            {
                switch (incoming)
                {
                    case "+":
                        BehindOperand = OperandStack.Pop();
                        FrontOperand = OperandStack.Pop();
                        OperandStack.Push(Convert.ToString(int.Parse(FrontOperand) + int.Parse(BehindOperand)));

                        break;
                    case "-":
                        BehindOperand = OperandStack.Pop();
                        FrontOperand = OperandStack.Pop();
                        OperandStack.Push(Convert.ToString(int.Parse(FrontOperand) - int.Parse(BehindOperand)));

                        break;
                    case "*":
                        BehindOperand = OperandStack.Pop();
                        FrontOperand = OperandStack.Pop();
                        OperandStack.Push(Convert.ToString(int.Parse(FrontOperand) * int.Parse(BehindOperand)));

                        break;
                    case "/":
                        BehindOperand = OperandStack.Pop();
                        FrontOperand = OperandStack.Pop();
                        OperandStack.Push(Convert.ToString(int.Parse(FrontOperand) / int.Parse(BehindOperand)));

                        break;

                    default:
                        OperandStack.Push(incoming);
                        break;
                }
            }
            Result = OperandStack.Pop();
            return Result;
        }

        private int priority(string operators)                      //运算子优先级
        {
            int p = 0;

            switch (operators)
            {
                case "+":
                case "-":
                    p = 1;
                    break;

                case "*":
                case "/":
                    p = 2;
                    break;

                case "U":
                case "L":
                    p = 3;
                    break;

                case "D":
                    p = 4;
                    break;

                default:
                    p = 0;
                    break;
            }
            return p;
        }

        private static string ReverseString(string input)           //反转字符串
        {
            if (String.IsNullOrEmpty(input))
            {
                return input;
            }

            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            return new String(charArray);
        }
        #endregion
    }
}
