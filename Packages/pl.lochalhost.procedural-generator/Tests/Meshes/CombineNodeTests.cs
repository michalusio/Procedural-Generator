using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Meshes;
using UnityEngine;

namespace lochalhost.procedural_generator.Editor.Tests.Meshes
{
    public class CombineNodeTests : NodeTestBase
    {
        [Test]
        public void ShouldProduceSameMeshData()
        {
            // Arrange
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            var node = new CombineNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = new List<object> { mesh };

            // Act
            node.Recalculate();

            // Assert
            Helper.AssertSameMeshData(mesh, node.Outputs[0].Value as Mesh);
        }

        [Test]
        public void ShouldProduceCombinedMeshData()
        {
            // Arrange
            var mesh1 = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            var mesh2 = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            var mesh3 = Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");
            var node = new CombineNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = new List<object> { mesh1, mesh2, mesh3 };

            // Act
            node.Recalculate();

            // Assert
            var checkMesh = new Mesh
            {
                vertices = mesh1.vertices.Concat(mesh2.vertices).Concat(mesh3.vertices).ToArray(),
                normals = mesh1.normals.Concat(mesh2.normals).Concat(mesh3.normals).ToArray(),
                triangles = mesh1.triangles.Concat(mesh2.triangles.Select(t => t + mesh1.vertexCount)).Concat(mesh3.triangles.Select(t => t + mesh1.vertexCount + mesh2.vertexCount)).ToArray(),
                uv = mesh1.uv.Concat(mesh2.uv).Concat(mesh3.uv).ToArray(),
            };
            Helper.AssertSameMeshData(checkMesh, node.Outputs[0].Value as Mesh);
        }

        [Test]
        public void ShouldNotChangeOutputWhenRecalculatedTwice()
        {
            // Arrange
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            var node = new CombineNode();
            Window.AddNode(node, new List<string> { });
            node.Inputs[0].Value = new List<object> { mesh };

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
            var node = new CombineNode();
            Window.AddNode(node, new List<string> { });
            Window.SaveChanges();

            Assert.AreEqual(0, Window.asset.Nodes[0].Data.Count);
        }
    }
}
