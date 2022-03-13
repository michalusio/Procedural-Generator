using System;
using System.Collections.Generic;
using NUnit.Framework;
using Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Math;
using static Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Math.RoundNode;

namespace lochalhost.procedural_generator.Editor.Tests.Math
{
    public class RoundNodeTests : NodeTestBase
    {
        [TestCase(RoundingMode.Ceil, 4.32f, 5)]
        [TestCase(RoundingMode.Floor, 4.32f, 4)]
        [TestCase(RoundingMode.Round, 4.32f, 4)]
        [TestCase(RoundingMode.Round, 4.72f, 5)]
        public void ShouldRound(RoundingMode roundingMode, float value, int result)
        {
            // Arrange
            var node = new RoundNode();
            Window.AddNode(node, new List<string> { roundingMode.ToString() });
            node.Inputs[0].Value = value;

            // Act
            node.Recalculate();

            // Assert
            Assert.IsInstanceOf<int>(node.Outputs[0].Value);
            Assert.AreEqual(result, node.Outputs[0].Value);
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
