using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using Test_Parser;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        public Func<double> Compile(string src)
        {
            var tree = BuildTree(src);
            LambdaExpression lambda = Expression.Lambda<Func<double>>(tree);
            var compiled = (Func<double>)lambda.Compile();

            return compiled;
        }

        [TestMethod]
        public void Expression_Add_Should_Success()
        {
            var compiled = Compile("1+5");

            Assert.AreEqual(compiled(), 6);
        }

        [TestMethod]
        public void FunctionDef_Call_Should_Success()
        {
            var value = Eval("f(x) = pow(x,2)+2");

            Assert.AreEqual(value, 0);
        }

        [TestMethod]
        public void FunctionDef_Should_Fail()
        {
            var value = Eval("x");

            Assert.AreEqual(value, 0);
        }

        [TestMethod]
        public void FunctionDef_Should_Success()
        {
            var value = Eval("f(x) =x+2");

            Assert.AreEqual(value, 0);
        }

        [TestMethod]
        public void Negate_GroupExpression_Complex_Should_Succeed()
        {
            var value = Eval("-(2*3+1)");

            Assert.AreEqual(value, -7);
        }

        [TestMethod]
        public void Negate_GroupExpression_Should_Succeed()
        {
            var value = Eval("-(2)");

            Assert.AreEqual(value, -2);
        }

        [TestMethod]
        public void ShrotPower_More_Should_Succeed()
        {
            var value = Eval("2²³");

            Assert.AreEqual(value, 8388608);
        }

        [TestMethod]
        public void ShrotPower_Negative_Should_Succeed()
        {
            var value = Eval("2⁻³");

            Assert.AreEqual(value, 0.125);
        }

        [TestMethod]
        public void ShrotPower_Should_Succeed()
        {
            var value = Eval("2²");

            Assert.AreEqual(value, 4);
        }

        private Expression BuildTree(string input)
        {
            var parser = new Parser();
            var tree = parser.Parse(input);
            return Evaluator.BuildExpression(tree, Evaluator.GlobalScope);
        }

        private double Eval(string input)
        {
            var parser = new Parser();
            var tree = parser.Parse(input);
            return Evaluator.Evaluate(tree);
        }
    }
}