using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Meshes;
using UnityEngine;

namespace lochalhost.procedural_generator.Editor.Tests.Meshes
{
    public class CloneNodeTests : NodeTestBase
    {
        [Test]
        public void ShouldProduceSameMeshDataWithCountZeroAndIdentityMatrix()
        {
            // Arrange
            var node = new CloneNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            node.Inputs[1].Value = 0;
            node.Inputs[2].Value = Matrix4x4.identity;

            // Act
            node.Recalculate();

            // Assert
            var mesh1 = node.Inputs[0].Value as Mesh;
            var mesh2 = node.Outputs[0].Value as Mesh;
            Helper.AssertSameMeshData(mesh1, mesh2);
        }

        [Test]
        public void ShouldProduceDuplicateMeshDataWithSomeCountAndIdentityMatrix()
        {
            const int CLONES = 7;
            // Arrange
            var node = new CloneNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            node.Inputs[1].Value = CLONES;
            node.Inputs[2].Value = Matrix4x4.identity;

            // Act
            node.Recalculate();

            // Assert
            var mesh1 = node.Inputs[0].Value as Mesh;
            var mesh2 = node.Outputs[0].Value as Mesh;
            Assert.AreEqual(mesh1.vertexCount * (CLONES + 1), mesh2.vertexCount);
            Assert.AreEqual(Enumerable.Range(0, CLONES + 1).SelectMany(i => mesh1.vertices).ToArray(), mesh2.vertices);
        }

        [Test]
        public void ShouldProduceNewMeshDataWithSomeCountAndNonIdentityMatrix()
        {
            const int CLONES = 2;
            // Arrange
            var matrix = Matrix4x4.Translate(Vector3.one);
            var node = new CloneNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            node.Inputs[1].Value = CLONES;
            node.Inputs[2].Value = matrix;

            // Act
            node.Recalculate();

            // Assert
            var mesh1 = node.Inputs[0].Value as Mesh;
            var vertices = mesh1.vertices;
            var mesh2 = node.Outputs[0].Value as Mesh;
            Assert.AreEqual(mesh1.vertexCount * (CLONES + 1), mesh2.vertexCount);
            Assert.AreEqual(
                vertices
                .Concat(vertices.Select(v => matrix.MultiplyPoint3x4(v)))
                .Concat(vertices.Select(v => matrix.MultiplyPoint3x4(matrix.MultiplyPoint3x4(v)))),
                
                mesh2.vertices
            );
        }

        [Test]
        public void ShouldProduceSameMeshDataWithCountZeroAndNonIdentityMatrix()
        {
            // Arrange
            var node = new CloneNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            node.Inputs[1].Value = 0;
            node.Inputs[2].Value = Matrix4x4.Scale(Vector3.one * 2);

            // Act
            node.Recalculate();

            // Assert
            var mesh1 = node.Inputs[0].Value as Mesh;
            var mesh2 = node.Outputs[0].Value as Mesh;
            Helper.AssertSameMeshData(mesh1, mesh2);
        }

        [Test]
        public void ShouldNotChangeOutputWhenRecalculatedTwice()
        {
            // Arrange
            var node = new CloneNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            node.Inputs[1].Value = 1;
            node.Inputs[2].Value = Matrix4x4.Translate(Vector3.one);

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
            var node = new CloneNode();
            Window.AddNode(node, new List<string> { });
            Window.SaveChanges();

            Assert.AreEqual(0, Window.asset.Nodes[0].Data.Count);
        }
    }
}
