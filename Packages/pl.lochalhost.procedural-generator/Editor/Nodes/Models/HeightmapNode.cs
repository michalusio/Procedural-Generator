using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Models
{
    [NodeName("Models/Heightmap")]
    public class HeightmapNode : Node
    {
        public HeightmapNode(): base("Heightmap")
        {
        }

        public override void Recalculate()
        {
            var width = Mathf.Max(Inputs[0].Value as int? ?? 1, 1);
            var height = Mathf.Max(Inputs[1].Value as int? ?? 1, 1);

            Outputs[0].Value = MakeMesh(width, height);
            MarkAsChanged();
        }

        private IEnumerable<int> GenerateTriangles(int w, int h, int height)
        {
            var index = w * height + h;
            yield return index; yield return index + 1; yield return index + height;
            yield return index + 1; yield return index + height + 1; yield return index + height;
        }

        private Mesh MakeMesh(int width, int height)
        {
            width++;
            height++;
            var floatHalfWidth = width / 2f;
            var floatHalfHeight = height / 2f;
            return new Mesh
            {
                vertices = Enumerable.Range(0, width).SelectMany(w =>
                {
                    var x = w - floatHalfWidth;
                    return Enumerable.Range(0, height).Select(h => new Vector3(x, 0, h - floatHalfHeight));
                }).ToArray(),
                triangles = Enumerable.Range(0, width - 1).SelectMany(w => Enumerable.Range(0, height - 1).SelectMany(h => GenerateTriangles(w, h, height))).ToArray(),
                normals = Enumerable.Repeat(Vector3.up, width * height).ToArray()
            };
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>
                {
                    new NodeIn("Width", typeof(int), false),
                    new NodeIn("Height", typeof(int), false)
                },
                new List<NodeOut>
                {
                    new NodeOut("Model", typeof(Mesh))
                    {
                        Value = MakeMesh(1, 1)
                    }
                }
            );
        }
    }
}
