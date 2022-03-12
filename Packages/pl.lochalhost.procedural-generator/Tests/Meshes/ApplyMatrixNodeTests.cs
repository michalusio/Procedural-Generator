using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Meshes;
using UnityEngine;

namespace lochalhost.procedural_generator.Editor.Tests.Meshes
{
    public class ApplyMatrixNodeTests : NodeTestBase
    {
        [Test]
        public void ShouldProduceSameMeshData()
        {
            // Arrange
            var node = new ApplyMatrixNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            node.Inputs[1].Value = Matrix4x4.identity;

            // Act
            node.Recalculate();

            // Assert
            Helper.AssertSameMeshData(node.Inputs[0].Value as Mesh, node.Outputs[0].Value as Mesh);
        }

        [Test]
        public void ShouldProduceModifiedMeshData()
        {
            // Arrange
            var matrix = Matrix4x4.Scale(Vector3.one * 2);
            var node = new ApplyMatrixNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            node.Inputs[1].Value = matrix;

            // Act
            node.Recalculate();

            // Assert
            var mesh1 = node.Inputs[0].Value as Mesh;
            var mesh2 = node.Outputs[0].Value as Mesh;
            Assert.AreEqual(mesh2.vertices, mesh1.vertices.Select(v => matrix.MultiplyPoint3x4(v)).ToArray());
        }

        [Test]
        public void ShouldNotChangeOutputWhenRecalculatedTwice()
        {
            // Arrange
            var node = new ApplyMatrixNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            node.Inputs[1].Value = Matrix4x4.Translate(Vector3.one);

            // Act
            node.Recalculate();
            var output = node.Outputs[0].Value;
            node.Recalculate();

            // Assert
            Helper.AssertSameMeshData(output as Mesh, node.Outputs[0].Value as Mesh);
        }

        [Test]
        public void ShouldSaveNothingWhenSaved()
        {
            var node = new ApplyMatrixNode();
            Window.AddNode(node, new List<string> { });
            Window.SaveChanges();

            Assert.AreEqual(0, Window.asset.Nodes[0].Data.Count);
        }
    }
}
