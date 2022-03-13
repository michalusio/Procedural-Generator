using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Meshes
{
    [NodeName("Meshes/Combine")]
    public class CombineNode : Node
    {
        public CombineNode(): base("Combine Models")
        {
        }

        public override void Recalculate()
        {
            if (Inputs[0].Value is IEnumerable list)
            {
                var models = list.OfType<Mesh>().ToList();
                var vertexCounts = new int[models.Count];
                for (int i = 1; i < models.Count; i++)
                {
                    vertexCounts[i] = vertexCounts[i - 1] + models[i - 1].vertexCount;
                }
                Outputs[0].Value = new Mesh
                {
                    vertices = models.SelectMany(m => m.vertices).ToArray(),
                    triangles = models.SelectMany((m, i) => m.triangles.Select(t => t + vertexCounts[i])).ToArray(),
                    normals = models.SelectMany((m, i) => m.normals).ToArray(),
                    uv = models.SelectMany((m, i) => m.uv).ToArray()
                };
                MarkAsChanged();
            }
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>()
                {
                    new NodeIn("Models", typeof(Mesh), true)
                },
                new List<NodeOut>
                {
                    new NodeOut("Result", typeof(Mesh))
                }
            );
        }
    }
}
