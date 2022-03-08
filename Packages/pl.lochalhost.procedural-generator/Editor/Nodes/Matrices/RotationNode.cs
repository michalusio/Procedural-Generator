using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Matrices
{
    [NodeName("Matrices/Rotation")]
    internal class RotationNode : Node
    {
        public RotationNode(): base("Rotation")
        {
        }

        public override void Recalculate()
        {
            var x = (Inputs[0].Value as float?) ?? 0;
            var y = (Inputs[1].Value as float?) ?? 0;
            var z = (Inputs[2].Value as float?) ?? 0;
            Outputs[0].Value = Matrix4x4.Rotate(Quaternion.Euler(x, y, z));
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
