using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Meshes
{
    [NodeName("Meshes/Normalize")]
    public class NormalizeNode : Node
    {
        public NormalizeNode(): base("Normalize Vertices")
        {
        }

        public override void Recalculate()
        {
            if (Inputs[0].Value is Mesh mesh)
            {
                var newMesh = new Mesh
                {
                    vertices = mesh.vertices.Select(v => v.normalized).ToArray(),
                    triangles = mesh.triangles,
                    normals = mesh.normals
                };
                newMesh.RecalculateNormals();
                Outputs[0].Value = newMesh;
                MarkAsChanged();
            }
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>()
                {
                    new NodeIn("Model", typeof(Mesh), false)
                },
                new List<NodeOut>
                {
                    new NodeOut("Result", typeof(Mesh))
                }
            );
        }
    }
}
