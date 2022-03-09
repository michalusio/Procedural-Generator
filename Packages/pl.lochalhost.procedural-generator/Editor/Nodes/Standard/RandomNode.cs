using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System;
using System.Collections.Generic;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Standard
{
    [NodeName("Standard/Random")]
    public class RandomNode : Node
    {
        private readonly Random rng;
        public RandomNode(): base("Random")
        {
            rng = new Random(GetHashCode());
        }

        private object lastSignal;

        public override void Recalculate()
        {
            var signal = Inputs[0].Value;
            if (lastSignal != signal)
            {
                Outputs[0].Value = (float)rng.NextDouble();
                MarkAsChanged();
                lastSignal = signal;
            }
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>
                {
                    new NodeIn("Signal", typeof(object), false)
                },
                new List<NodeOut>
                {
                    new NodeOut("Value", typeof(float))
                    {
                        Value = 0f
                    }
                }
            );
        }
    }
}
