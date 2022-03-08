using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Math
{
    [NodeName("Matrices/Combine")]
    internal class CombineNode : Node
    {
        public CombineNode(): base("Combine Matrices")
        {
        }

        public override void Recalculate()
        {
            var newValue = (Inputs[0].Value as IEnumerable)
                .OfType<Matrix4x4>()
                .Aggregate((acc, m) => m * acc);
            if (newValue != (Matrix4x4)Outputs[0].Value)
            {
                Outputs[0].Value = newValue;
                MarkAsChanged();
            }
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>()
                {
                    new NodeIn("Matrices", typeof(Matrix4x4), true)
                },
                new List<NodeOut>
                {
                    new NodeOut("Result", typeof(Matrix4x4))
                    {
                        Value = Matrix4x4.identity
                    }
                }
            );
        }
    }
}
