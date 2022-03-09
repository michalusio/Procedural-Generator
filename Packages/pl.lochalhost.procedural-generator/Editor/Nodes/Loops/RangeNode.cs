using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Loops
{
    [NodeName("Loops/Range")]
    public class RangeNode : Node
    {
        public RangeNode(): base("Range")
        {
        }

        public override void Recalculate()
        {
            var count = (Inputs[0].Value as int?) ?? 0;
            Outputs[0].Value = Enumerable.Range(0, count).ToList().AsReadOnly();
            MarkAsChanged();
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>
                {
                    new NodeIn("Count", typeof(int), false)
                },
                new List<NodeOut>
                {
                    new NodeOut("Range", typeof(IEnumerable<int>))
                    {
                        Value = new List<int>().AsReadOnly()
                    }
                }
            );
        }
    }
}
