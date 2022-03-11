using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Math;
using UnityEngine;

namespace lochalhost.procedural_generator.Editor.Tests.Matrices
{
    public class CombineNodeTests : NodeTestBase
    {
        [Test]
        public void ShouldProduceSameMatrix()
        {
            // Arrange
            var matrix = Matrix4x4.Scale(new Vector3(0.7f, 0.5f, 1));
            var node = new CombineNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = new List<object> { matrix };

            // Act
            node.Recalculate();

            // Assert
            Assert.AreEqual(matrix, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldMultiplyMatrices()
        {
            // Arrange
            var matrix1 = Matrix4x4.Scale(new Vector3(0.7f, 0.5f, 1));
            var matrix2 = Matrix4x4.Rotate(Quaternion.AngleAxis(0.9f, Vector3.up));
            var node = new CombineNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = new List<object> { matrix1, matrix2 };

            // Act
            node.Recalculate();

            // Assert
            Assert.AreEqual(matrix2 * matrix1, node.Outputs[0].Value);
        }

        [Test]
        public void ShouldMultiplyALotOfMatrices()
        {
            // Arrange
            var matrices = Enumerable.Range(0, 50).Select(i => Matrix4x4.Rotate(Quaternion.Euler(i, i * 2, 0))).OfType<object>().ToList();
            var node = new CombineNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = matrices;

            // Act
            node.Recalculate();

            // Assert
            Assert.AreEqual(matrices.OfType<Matrix4x4>().Aggregate((acc, m) => m * acc), node.Outputs[0].Value);
        }

        [Test]
        public void ShouldNotChangeOutputWhenRecalculatedTwice()
        {
            // Arrange
            var node = new CombineNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = new List<object> { Matrix4x4.Translate(Vector3.one), Matrix4x4.Scale(new Vector3(0.7f, 0.5f, 1)) };

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
            var node = new CombineNode();
            Window.AddNode(node, new List<string> { });
            Window.SaveChanges();

            Assert.AreEqual(0, Window.asset.Nodes[0].Data.Count);
        }
    }
}
