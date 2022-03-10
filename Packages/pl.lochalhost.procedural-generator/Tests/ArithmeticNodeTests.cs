using System;
using System.Collections.Generic;
using NUnit.Framework;
using Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Math;
using UnityEngine;
using static Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Math.ArithmeticNode;

namespace lochalhost.procedural_generator.Editor.Tests
{
    public class ArithmeticNodeTests : NodeTestBase
    {
        [Test]
        public void ShouldAddTwoNumbers()
        {
            // Arrange
            var node = new ArithmeticNode();
            Window.AddNode(node, new List<string> { Operation.Addition.ToString() });
            node.Inputs[0].Value = 4f;
            node.Inputs[1].Value = new List<object> { 7f };

            // Act
            node.Recalculate();

            // Assert
            Assert.AreEqual(11f, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldSubtractTwoNumbers()
        {
            // Arrange
            var node = new ArithmeticNode();
            Window.AddNode(node, new List<string> { Operation.Subtraction.ToString() });
            node.Inputs[0].Value = 4f;
            node.Inputs[1].Value = new List<object> { 7f };

            // Act
            node.Recalculate();

            // Assert
            Assert.AreEqual(-3f, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldMultiplyTwoNumbers()
        {
            // Arrange
            var node = new ArithmeticNode();
            Window.AddNode(node, new List<string> { Operation.Multiplication.ToString() });
            node.Inputs[0].Value = 4f;
            node.Inputs[1].Value = new List<object> { 7f };

            // Act
            node.Recalculate();

            // Assert
            Assert.AreEqual(28f, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldDivideTwoNumbers()
        {
            // Arrange
            var node = new ArithmeticNode();
            Window.AddNode(node, new List<string> { Operation.Division.ToString() });
            node.Inputs[0].Value = 4f;
            node.Inputs[1].Value = new List<object> { 7f };

            // Act
            node.Recalculate();

            // Assert
            Assert.AreEqual(4/7f, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldGetSquareRoot()
        {
            // Arrange
            var node = new ArithmeticNode();
            Window.AddNode(node, new List<string> { Operation.SquareRoot.ToString() });
            node.Inputs[0].Value = 4f;
            node.Inputs[1].Value = new List<object> { 7f };

            // Act
            node.Recalculate();

            // Assert
            Assert.AreEqual(2f, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldGetSine()
        {
            // Arrange
            var node = new ArithmeticNode();
            Window.AddNode(node, new List<string> { Operation.Sin.ToString() });
            node.Inputs[0].Value = Mathf.PI/2;
            node.Inputs[1].Value = new List<object> { 7f };

            // Act
            node.Recalculate();

            // Assert
            Assert.AreEqual(1f, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldGetCosine()
        {
            // Arrange
            var node = new ArithmeticNode();
            Window.AddNode(node, new List<string> { Operation.Cos.ToString() });
            node.Inputs[0].Value = Mathf.PI;
            node.Inputs[1].Value = new List<object> { 7f };

            // Act
            node.Recalculate();

            // Assert
            Assert.AreEqual(-1f, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldGetTangens()
        {
            // Arrange
            var node = new ArithmeticNode();
            Window.AddNode(node, new List<string> { Operation.Tan.ToString() });
            node.Inputs[0].Value = Mathf.PI / 4;
            node.Inputs[1].Value = new List<object> { 7f };

            // Act
            node.Recalculate();

            // Assert
            Assert.AreEqual(1f, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldGetArcusTangensUp()
        {
            // Arrange
            var node = new ArithmeticNode();
            Window.AddNode(node, new List<string> { Operation.Atan.ToString() });
            node.Inputs[0].Value = 1f;
            node.Inputs[1].Value = new List<object> { 0f };

            // Act
            node.Recalculate();

            // Assert
            Assert.AreEqual(Mathf.PI / 2, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldGetArcusTangensRight()
        {
            // Arrange
            var node = new ArithmeticNode();
            Window.AddNode(node, new List<string> { Operation.Atan.ToString() });
            node.Inputs[0].Value = 0f;
            node.Inputs[1].Value = new List<object> { 1f };

            // Act
            node.Recalculate();

            // Assert
            Assert.AreEqual(0f, node.Outputs[0].Value);
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
