using System.Collections.Generic;
using NUnit.Framework;
using Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Meshes;
using UnityEngine;

namespace lochalhost.procedural_generator.Editor.Tests.Meshes
{
    public class SubdivisionNodeTests : NodeTestBase
    {
        [Test]
        public void ShouldProduceSameVertices()
        {
            // Arrange
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            var node = new SubdivisionNode();
            Window.AddNode(node, new List<string> { "0", "Simple" });
            node.Inputs[0].Value = mesh;

            // Act
            node.Recalculate();

            // Assert
            var outMesh = node.Outputs[0].Value as Mesh;
            Helper.AssertSameMeshData(mesh, outMesh);
        }

        [Test]
        public void ShouldProduceSubdividedMeshData()
        {
            // Arrange
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            var node = new SubdivisionNode();
            Window.AddNode(node, new List<string> { "1", "Simple" });
            node.Inputs[0].Value = mesh;

            // Act
            node.Recalculate();

            // Assert
            var outMesh = node.Outputs[0].Value as Mesh;
            Assert.AreEqual(mesh.triangles.Length * 4, outMesh.triangles.Length);
            Assert.AreEqual(outMesh.vertexCount, outMesh.uv.Length);
            Assert.AreEqual(outMesh.vertexCount, outMesh.normals.Length);
        }

        [Test]
        public void ShouldProduceSubdividedALotMeshData()
        {
            // Arrange
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            var node = new SubdivisionNode();
            Window.AddNode(node, new List<string> { "4", "Simple" });
            node.Inputs[0].Value = mesh;

            // Act
            node.Recalculate();

            // Assert
            var outMesh = node.Outputs[0].Value as Mesh;
            Assert.AreEqual(mesh.triangles.Length * 256, outMesh.triangles.Length);
            Assert.AreEqual(outMesh.vertexCount, outMesh.uv.Length);
            Assert.AreEqual(outMesh.vertexCount, outMesh.normals.Length);
        }

        [Test]
        public void ShouldNotChangeOutputWhenRecalculatedTwice()
        {
            // Arrange
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            var node = new SubdivisionNode();
            Window.AddNode(node, new List<string> { "1", "Simple" });
            node.Inputs[0].Value = mesh;

            // Act
            node.Recalculate();
            var output = node.Outputs[0].Value;
            node.Recalculate();

            // Assert
            Helper.AssertSameMeshData(output as Mesh, node.Outputs[0].Value as Mesh);
        }

        [Test]
        public void ShouldSaveTwoValuesWhenSaved()
        {
            var node = new SubdivisionNode();
            Window.AddNode(node, new List<string> { "4", "Simple" });
            Window.SaveChanges();

            Assert.AreEqual(2, Window.asset.Nodes[0].Data.Count);
        }
    }
}
