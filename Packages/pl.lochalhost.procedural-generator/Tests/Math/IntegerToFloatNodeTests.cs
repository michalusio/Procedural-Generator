using System.Collections.Generic;
using NUnit.Framework;
using Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Math;

namespace lochalhost.procedural_generator.Editor.Tests.Math
{
    public class IntegerToFloatNodeTests : NodeTestBase
    {
        [Test]
        public void ShouldConvertIntToFloat()
        {
            // Arrange
            var node = new IntegerToFloatNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = 12;

            // Act
            node.Recalculate();

            // Assert
            Assert.IsInstanceOf<float>(node.Outputs[0].Value);
            Assert.AreEqual(12f, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldNotChangeOutputWhenRecalculatedTwice()
        {
            // Arrange
            var node = new IntegerToFloatNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = 12;

            // Act
            node.Recalculate();
            var output = node.Outputs[0].Value;
            node.Recalculate();

            // Assert
            Assert.AreEqual(output, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldSaveNothingWhenSaved()
        {
            var node = new IntegerToFloatNode();
            Window.AddNode(node, new List<string> { });
            Window.SaveChanges();

            Assert.AreEqual(0, Window.asset.Nodes[0].Data.Count);
        }
    }
}
