using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Math;
using UnityEngine;
using static Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Math.ArithmeticNode;

namespace lochalhost.procedural_generator.Editor.Tests.Math
{
    public class ArithmeticNodeTests : NodeTestBase
    {
        [TestCase(Operation.Addition, 11f, 4f, 7f)]
        [TestCase(Operation.Subtraction, -3f, 4f, 7f)]
        [TestCase(Operation.Multiplication, 28f, 4f, 7f)]
        [TestCase(Operation.Division, 4/7f, 4f, 7f)]
        [TestCase(Operation.SquareRoot, 2f, 4f, 7f)]
        [TestCase(Operation.Sin, 1f, Mathf.PI / 2, 7f)]
        [TestCase(Operation.Cos, -1f, Mathf.PI, 7f)]
        [TestCase(Operation.Tan, 1f, Mathf.PI / 4, 7f)]
        [TestCase(Operation.Atan, Mathf.PI / 2, 1f, 0f)]
        [TestCase(Operation.Atan, 0f, 0f, 1f)]
        public void ShouldCalculate(Operation operation, float result, float value1, params float[] value2)
        {
            // Arrange
            var node = new ArithmeticNode();
            Window.AddNode(node, new List<string> { operation.ToString() });
            node.Inputs[0].Value = value1;
            node.Inputs[1].Value = value2.OfType<object>().ToList();

            // Act
            node.Recalculate();

            // Assert
            Assert.AreEqual(result, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldNotChangeSquareRootWithDifferentSecondArgument()
        {
            // Arrange
            var node = new ArithmeticNode();
            Window.AddNode(node, new List<string> { Operation.SquareRoot.ToString() });
            node.Inputs[0].Value = 4f;
            node.Inputs[1].Value = new List<object> { 7f };

            // Act
            node.Recalculate();
            var output = node.Outputs[0].Value;
            node.Inputs[1].Value = new List<object> { 1f };
            node.Recalculate();

            // Assert
            Assert.AreEqual(output, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldNotChangeOutputWhenRecalculatedTwice()
        {
            // Arrange
            var node = new ArithmeticNode();
            Window.AddNode(node, new List<string> { Operation.Addition.ToString() });
            node.Inputs[0].Value = 1f;
            node.Inputs[1].Value = new List<object> { 2f };

            // Act
            node.Recalculate();
            var output = node.Outputs[0].Value;
            node.Recalculate();

            // Assert
            Assert.AreEqual(output, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldThrowWhenGivenInvalidOperation()
        {
            // Arrange
            var node = new ArithmeticNode();
            
            // Act && Assert
            Assert.Throws(typeof(ArgumentException), () =>
            {
                Window.AddNode(node, new List<string> { "This is not an operation" });
            });
        }

        [Test]
        public void ShouldSaveTheOperationWhenSaved()
        {
            var node = new ArithmeticNode();
            Window.AddNode(node, new List<string> { Operation.Division.ToString() });
            Window.SaveChanges();

            Assert.AreEqual(Operation.Division.ToString(), Window.asset.Nodes[0].Data[0]);
        }
    }
}
