using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Matrices
{
    [NodeName("Matrices/Scale")]
    public class ScaleNode : Node
    {
        public ScaleNode(): base("Scale")
        {
        }

        public override void Recalculate()
        {
            var x = (Inputs[0].Value as float?) ?? 1;
            var y = (Inputs[1].Value as float?) ?? 1;
            var z = (Inputs[2].Value as float?) ?? 1;
            Outputs[0].Value = Matrix4x4.Scale(new Vector3(x, y, z));
            MarkAsChanged();
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>()
                {
                    new NodeIn("X", typeof(float), false),
                    new NodeIn("Y", typeof(float), false),
                    new NodeIn("Z", typeof(float), false),
                },
                new List<NodeOut>
                {
                    new NodeOut("Matrix", typeof(Matrix4x4))
                    {
                        Value = Matrix4x4.identity
                    }
                }
            );
        }
    }
}
