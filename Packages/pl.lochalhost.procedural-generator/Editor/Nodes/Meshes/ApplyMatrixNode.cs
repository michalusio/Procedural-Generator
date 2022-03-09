using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Meshes
{
    [NodeName("Meshes/Apply Matrix")]
    public class ApplyMatrixNode : Node
    {
        public ApplyMatrixNode(): base("Apply Matrix")
        {
        }

        public override void Recalculate()
        {
            var model = Inputs[0].Value as Mesh;
            var matrix = (Inputs[1].Value as Matrix4x4?) ?? Matrix4x4.identity;
            if (model != null)
            {
                Outputs[0].Value = new Mesh
                {
                    vertices = model.vertices.Select(v => matrix.MultiplyPoint3x4(v)).ToArray(),
                    triangles = model.triangles,
                    normals = model.normals
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
                    new NodeIn("Matrix", typeof(Matrix4x4), false),
                },
                new List<NodeOut>
                {
                    new NodeOut("Result", typeof(Mesh))
                }
            );
        }
    }
}
