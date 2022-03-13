using System;
using System.Collections.Generic;
using NUnit.Framework;
using Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Meshes;
using UnityEngine;

namespace lochalhost.procedural_generator.Editor.Tests.Meshes
{
    public class SubdivisionNodeTests : NodeTestBase
    {
        [TestCase("0", 24, 12)]
        [TestCase("1", 54, 48)]
        [TestCase("3", 486, 768)]
        [TestCase("4", 1734, 3072)]
        public void ShouldProduceSubdividedMeshData(string divisions, int resultingVertices, int resultingTriangles)
        {
            // Arrange
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            var node = new SubdivisionNode();
            Window.AddNode(node, new List<string> { divisions, "Simple" });
            node.Inputs[0].Value = mesh;

            // Act
            node.Recalculate();

            // Assert
            var outMesh = node.Outputs[0].Value as Mesh;
            Assert.AreEqual(resultingVertices, outMesh.vertexCount);
            Assert.AreEqual(resultingVertices, outMesh.uv.Length);
            Assert.AreEqual(resultingVertices, outMesh.normals.Length);
            Assert.AreEqual(resultingTriangles, outMesh.triangles.Length / 3);
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
        public void ShouldThrowWhenGivenInvalidRoundingMode()
        {
            // Arrange
            var node = new SubdivisionNode();

            // Act && Assert
            Assert.Throws(typeof(ArgumentException), () =>
            {
                Window.AddNode(node, new List<string> { "0", "This is not a subdivision mode" });
            });
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
