using System.Collections.Generic;
using NUnit.Framework;
using Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Matrices;
using UnityEngine;

namespace lochalhost.procedural_generator.Editor.Tests.Matrices
{
    public class TranslationNodeTests : NodeTestBase
    {
        [Test]
        public void ShouldProduceCorrectMatrix()
        {
            // Arrange
            var node = new TranslationNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = 1f;
            node.Inputs[1].Value = 3f;
            node.Inputs[2].Value = 5f;

            // Act
            node.Recalculate();

            // Assert
            Assert.AreEqual(Matrix4x4.Translate(new Vector3(1f, 3f, 5f)), node.Outputs[0].Value);
        }

        [Test]
        public void ShouldTreatUnassignedAsZero()
        {
            // Arrange
            var node = new TranslationNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = 5f;

            // Act
            node.Recalculate();

            // Assert
            Assert.AreEqual(Matrix4x4.Translate(new Vector3(5f, 0f, 0f)), node.Outputs[0].Value);
        }

        [Test]
        public void ShouldNotChangeOutputWhenRecalculatedTwice()
        {
            // Arrange
            var node = new TranslationNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = 5f;
            node.Inputs[1].Value = 3f;
            node.Inputs[2].Value = 7f;

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
            var node = new TranslationNode();
            Window.AddNode(node, new List<string> { });
            Window.SaveChanges();

            Assert.AreEqual(0, Window.asset.Nodes[0].Data.Count);
        }
    }
}
