using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections.Generic;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Math
{
    [NodeName("Math/Integer To Float")]
    public class IntegerToFloatNode : Node
    {
        public IntegerToFloatNode(): base("I2F")
        {
        }

        public override void Recalculate()
        {
            var value = (Inputs[0].Value as int?) ?? 0;
            Outputs[0].Value = (float)value;
            MarkAsChanged();
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>()
                {
                    new NodeIn("In", typeof(int), false)
                },
                new List<NodeOut>
                {
                    new NodeOut("Out", typeof(float))
                    {
                        Value = 0f
                    }
                }
            );
        }
    }
}
