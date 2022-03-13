using System.Collections.Generic;
using NUnit.Framework;
using Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Meshes;
using UnityEngine;

namespace lochalhost.procedural_generator.Editor.Tests.Meshes
{
    public class NormalizeNodeTests : NodeTestBase
    {
        [Test]
        public void ShouldProduceSameVertices()
        {
            // Arrange
            var mesh = new Mesh {
                vertices = new Vector3[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right }
            };
            var node = new NormalizeNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = mesh;

            // Act
            node.Recalculate();

            // Assert
            var outMesh = node.Outputs[0].Value as Mesh;
            Helper.AreEqual(mesh.vertices, outMesh.vertices);
        }

        [Test]
        public void ShouldProduceNormalizedVerticesMeshData()
        {
            // Arrange
            var mesh = Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");
            var node = new NormalizeNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = mesh;

            // Act
            node.Recalculate();

            // Assert
            var outMesh = node.Outputs[0].Value as Mesh;
            foreach(var v in outMesh.vertices)
            {
                Assert.AreEqual(1, v.sqrMagnitude, Helper.FLOAT_DELTA);
            }
        }

        [Test]
        public void ShouldNotChangeOutputWhenRecalculatedTwice()
        {
            // Arrange
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            var node = new NormalizeNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = mesh;

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
            var node = new NormalizeNode();
            Window.AddNode(node, new List<string> { });
            Window.SaveChanges();

            Assert.AreEqual(0, Window.asset.Nodes[0].Data.Count);
        }
    }
}
