using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Meshes
{
    [NodeName("Meshes/Clone")]
    public class CloneNode : Node
    {
        public CloneNode(): base("Clone")
        {
        }

        public override void Recalculate()
        {
            var model = Inputs[0].Value as Mesh;
            var clones = (Inputs[1].Value as int?) ?? 0;
            var matrix = (Inputs[2].Value as Matrix4x4?) ?? Matrix4x4.identity;
            if (model != null)
            {
                var startingVertices = model.vertices;
                var startingTriangles = model.triangles;
                var startingNormals = model.normals;
                var matrices = new Matrix4x4[clones + 1];
                matrices[0] = Matrix4x4.identity;
                for (int i = 1; i < matrices.Length; i++)
                {
                    matrices[i] = matrices[i - 1] * matrix;
                }

                Outputs[0].Value = new Mesh
                {
                    vertices = matrices.SelectMany(m => startingVertices.Select(v => m.MultiplyPoint3x4(v))).ToArray(),
                    triangles = matrices.SelectMany((_, i) => startingTriangles.Select(t => t + i * startingVertices.Length)).ToArray(),
                    normals = matrices.SelectMany((m, i) => startingNormals.Select(n => m.MultiplyVector(n))).ToArray()
                };
                MarkAsChanged();
            }
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>()
                {
                    new NodeIn("Model", typeof(Mesh), false),
                    new NodeIn("Clones", typeof(int), false),
                    new NodeIn("Matrix per clone", typeof(Matrix4x4), false),
                },
                new List<NodeOut>
                {
                    new NodeOut("Result", typeof(Mesh))
                }
            );
        }
    }
}
