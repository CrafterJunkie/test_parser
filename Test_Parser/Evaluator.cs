using GrandTextAdventure.Core.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Test_Parser
{
    public static class Evaluator
    {
        public static readonly Scope GlobalScope = new();
        private static Dictionary<string, Function> _definedFunctions = new();
        private static Dictionary<string, Delegate> _importedFunctions = new();

        static Evaluator()
        {
            AddVariable("pi", new Number(3.14f));
            AddVariable("e", new Number(2.718f));

            /* _functions.Add("log", new Func<double, double>(_ =>
             {
                 return (double)Math.Log10(_);
             }));
             _functions.Add("sin", new Func<double, double>(_ =>
             {
                 return (double)Math.Sin(_);
             }));
             _functions.Add("cos", new Func<double, double>(_ =>
             {
                 return (double)Math.Cos(_);
             }));
             _functions.Add("tan", new Func<double, double>(_ =>
             {
                 return (double)Math.Tan(_);
             }));*/

            ImportFunctions(typeof(Math));
        }

        public static Expression BuildExpression(SyntaxNode node, Scope scope)
        {
            switch (node)
            {
                case BlockNode blkNode:
                    {
                        var exprs = new List<Expression>();
                        foreach (var childNode in blkNode.Children)
                        {
                            exprs.Add(BuildExpression(childNode, scope));
                        }

                        return Expression.Block(exprs);
                    }
                case Number numNode:
                    return Expression.Constant(numNode.Value);

                case ShortPowerNode shortPowerNode:
                    var exp = BuildExpression(((Number)shortPowerNode.Power1), scope);
                    var mi = typeof(Math).GetMethod("Pow");

                    var args = new List<Expression>();
                    args.Add(BuildExpression(((Number)shortPowerNode.Base1), scope));
                    if (shortPowerNode.IsNegative)
                    {
                        args.Add(Expression.Negate(exp));
                    }
                    else
                    {
                        args.Add(exp);
                    }

                    return Expression.Call(mi, args);

                case BinaryExpression binaryExpr:
                    var left = BuildExpression(binaryExpr.Left, scope);
                    var right = BuildExpression(binaryExpr.Right, scope);

                    switch (binaryExpr.Op)
                    {
                        case Operator.Addition:
                            return Expression.Add(left, right);

                        case Operator.Subtraktion:
                            return Expression.Subtract(left, right);

                        case Operator.Multiplication:
                            return Expression.Multiply(left, right);

                        case Operator.Division:
                            return Expression.Divide(left, right);

                        case Operator.Power:
                            return Expression.Power(left, right);

                        case Operator.Modulo:
                            return Expression.Modulo(left, right);

                        default:
                            return Expression.Constant(0);
                    }
                case GroupExpression groupNode:
                    if (groupNode.IsNegated)
                    {
                        return Expression.Negate(BuildExpression(groupNode.Expression, scope));
                    }
                    else
                    {
                        return BuildExpression(groupNode.Expression, scope);
                    }
                case AbsoluteValueExpression absoluteValueNode:
                    {
                        var expr = BuildExpression(absoluteValueNode.Expression, scope);

                        return Expression.Call(typeof(Math).GetMethod("Abs"), expr);
                    }
                case NameExpression nameExpr:
                    // ToDo: Get Variable from scope

                    break;
            }

            return null;
        }

        public static double Evaluate(SyntaxNode node)
        {
            return Evaluate(node, GlobalScope);
        }

        public static double Evaluate(SyntaxNode node, Scope scope)
        {
            switch (node)
            {
                case Number numNode:
                    return numNode.Value;

                case ShortPowerNode shortPowerNode:
                    var exp = ((Number)shortPowerNode.Power1).Value;
                    return Math.Pow(((Number)shortPowerNode.Base1).Value, shortPowerNode.IsNegative ? -exp : exp);

                case GroupExpression groupNode:
                    double groupValue = Evaluate(groupNode.Expression, scope);
                    if (groupNode.IsNegated)
                    {
                        groupValue = -groupValue;
                    }

                    return groupValue;

                case AbsoluteValueExpression absoluteValueNode:
                    {
                        var value = Evaluate(absoluteValueNode.Expression, scope);

                        return Math.Abs(value);
                    }

                case FuncCallExpression funcCallNode:
                    if (_importedFunctions.ContainsKey(funcCallNode.Name))
                    {
                        var func = _importedFunctions[funcCallNode.Name];

                        var args = ((BlockNode)funcCallNode.Args).Children.Select(_ => Evaluate(_, scope));

                        return (double)func.DynamicInvoke(args.ToArray());
                    }
                    else if (_definedFunctions.ContainsKey(funcCallNode.Name))
                    {
                        var func = _definedFunctions[funcCallNode.Name];
                        var funcScope = GlobalScope.CreateScope();

                        var args = ((BlockNode)funcCallNode.Args).Children.ToArray();

                        for (int i = 0; i < args.Length; i++)
                        {
                            funcScope.Add(func.Parameters[i], args[i]);
                        }

                        return func.Invoke(funcScope);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Function '{0}' not found!", funcCallNode.Name);
                        Console.ResetColor();
                    }
                    break;

                case FunctionDefinitionNode funcDefNode:
                    var function = new Function { Body = funcDefNode.Body, Parameters = funcDefNode.Args };

                    if (!_definedFunctions.ContainsKey(funcDefNode.Name))
                    {
                        _definedFunctions.Add(funcDefNode.Name, function);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Function '{0}' already exists", funcDefNode.Name);
                        Console.ResetColor();
                    }

                    return 0;

                case NameExpression neNode:

                    return Evaluate(scope.GetValue(neNode.Identifier), scope);

                case BinaryExpression binNode:
                    {
                        var left = Evaluate(binNode.Left, scope);
                        var right = Evaluate(binNode.Right, scope);

                        switch (binNode.Op)
                        {
                            case Operator.Addition:
                                return left + right;

                            case Operator.Subtraktion:
                                return left - right;

                            case Operator.Multiplication:
                                return left * right;

                            case Operator.Power:
                                return (int)Math.Pow(left, right);

                            case Operator.Modulo:
                                return left % right;

                            default:
                                break;
                        }

                        break;
                    }

                case VariableDefinitionNode defNode:
                    AddVariable(defNode.Name, defNode.Expr);
                    break;

                case BlockNode blkNode:
                    {
                        foreach (var childNode in blkNode.Children)
                        {
                            return Evaluate(childNode, scope);
                        }

                        break;
                    }
            }

            return 0;
        }

        public static void ImportFunctions(Type type)
        {
            var methods = type.GetMethods().Where(_ => _.IsStatic);

            foreach (var m in methods)
            {
                if (!_importedFunctions.ContainsKey(m.Name.ToLower()))
                {
                    _importedFunctions.Add(m.Name.ToLower(), new Func<double[], double>(_ =>
                    {
                        return (double)m.Invoke(null, _.Cast<object>().ToArray());
                    }));
                }
            }
        }

        internal static void AddVariable(string name, SyntaxNode value)
        {
            GlobalScope.Add(name, value);
        }
    }
}