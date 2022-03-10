using System;
using System.Collections.Generic;
using NUnit.Framework;
using Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Math;
using UnityEngine;
using static Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Math.RoundNode;

namespace lochalhost.procedural_generator.Editor.Tests
{
    public class RoundNodeTests : NodeTestBase
    {
        [Test]
        public void ShouldRoundCeil()
        {
            // Arrange
            var node = new RoundNode();
            Window.AddNode(node, new List<string> { RoundingMode.Ceil.ToString() });
            node.Inputs[0].Value = 4.32f;

            // Act
            node.Recalculate();

            // Assert
            Assert.IsInstanceOf<int>(node.Outputs[0].Value);
            Assert.AreEqual(5, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldRoundFloor()
        {
            // Arrange
            var node = new RoundNode();
            Window.AddNode(node, new List<string> { RoundingMode.Floor.ToString() });
            node.Inputs[0].Value = 4.32f;

            // Act
            node.Recalculate();

            // Assert
            Assert.IsInstanceOf<int>(node.Outputs[0].Value);
            Assert.AreEqual(4, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldRound1()
        {
            // Arrange
            var node = new RoundNode();
            Window.AddNode(node, new List<string> { RoundingMode.Round.ToString() });
            node.Inputs[0].Value = 4.32f;

            // Act
            node.Recalculate();

            // Assert
            Assert.IsInstanceOf<int>(node.Outputs[0].Value);
            Assert.AreEqual(4, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldRound2()
        {
            // Arrange
            var node = new RoundNode();
            Window.AddNode(node, new List<string> { RoundingMode.Round.ToString() });
            node.Inputs[0].Value = 4.72f;

            // Act
            node.Recalculate();

            // Assert
            Assert.IsInstanceOf<int>(node.Outputs[0].Value);
            Assert.AreEqual(5, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldNotChangeOutputWhenRecalculatedTwice()
        {
            // Arrange
            var node = new RoundNode();
            Window.AddNode(node, new List<string> { RoundingMode.Round.ToString() });
            node.Inputs[0].Value = 1.5f;

            // Act
            node.Recalculate();
            var output = node.Outputs[0].Value;
            node.Recalculate();

            // Assert
            Assert.AreEqual(output, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldThrowWhenGivenInvalidRoundingMode()
        {
            // Arrange
            var node = new RoundNode();
            
            // Act && Assert
            Assert.Throws(typeof(ArgumentException), () =>
            {
                Window.AddNode(node, new List<string> { "This is not a rounding mode" });
            });
        }

        [Test]
        public void ShouldSaveTheRoundingModeWhenSaved()
        {
            var node = new RoundNode();
            Window.AddNode(node, new List<string> { RoundingMode.Floor.ToString() });
            Window.SaveChanges();

            Assert.AreEqual(RoundingMode.Floor.ToString(), Window.asset.Nodes[0].Data[0]);
        }
    }
}
