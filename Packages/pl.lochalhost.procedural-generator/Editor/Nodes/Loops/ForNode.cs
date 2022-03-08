using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Loops
{
    internal struct LoopLink
    {
        public int Index;
        public bool LoopEnd;
    }

    [NodeName("Loops/For/Start")]
    internal class ForStartNode : Node
    {
        public ForStartNode(): base("For Start")
        {
        }

        public override void Recalculate()
        {
            if (Inputs[0].Value is IEnumerable items)
            {
                int i = 0;
                foreach (var item in items)
                {
                    Outputs[0].Value = new LoopLink
                    {
                        Index = i,
                        LoopEnd = false
                    };
                    Outputs[1].Value = item;
                    MarkAsChanged();
                    i++;
                }
                Outputs[0].Value = new LoopLink
                {
                    Index = -1,
                    LoopEnd = true
                };
                MarkAsChanged();
            }
        }

        protected internal override void OnInputLinked(Connection con)
        {
            Inputs[0].Type = con.From.Type;
            Outputs[1].Type = con.From.Type.GetGenericArguments().FirstOrDefault() ?? typeof(object);
        }

        protected internal override void OnInputUnlinked(Connection connection)
        {
            Inputs[0].Type = typeof(object);
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>
                {
                    new NodeIn("Values", typeof(object), false)
                },
                new List<NodeOut>
                {
                    new NodeOut("Loop Link", typeof(LoopLink)),
                    new NodeOut("Loop Item", typeof(object))
                }
            );
        }
    }

    [NodeName("Loops/For/End")]
    internal class ForEndNode : Node
    {
        private readonly Dictionary<int, object> items = new Dictionary<int, object>();

        public ForEndNode() : base("For End")
        {
        }

        private object lastItem;
        public override void Recalculate()
        {
            var link = (Inputs[0].Value as LoopLink?) ?? new LoopLink { LoopEnd = false, Index = -1 };
            var item = Inputs[1].Value;
            if (link.Index > -1)
            {
                if (link.Index == 0) items.Clear();
                items[link.Index] = item;
            }
            if (link.LoopEnd)
            {
                if (lastItem != item)
                {
                    ForceReloop();
                    return;
                }
                Outputs[0].Value = ToEnumerableOf(items.OrderBy(kv => kv.Key).Select(kv => kv.Value).ToList(), Inputs[1].Type);
                MarkAsChanged();
            }
            lastItem = item;
        }

        private IEnumerable ToEnumerableOf(List<object> collection, Type type)
        {
            return (IEnumerable)typeof(Enumerable).GetMethod(nameof(Enumerable.OfType)).MakeGenericMethod(type).Invoke(null, new object[] { collection });
        }

        private void ForceReloop()
        {
            Inputs[0].Connections.Select(c => c.From?.Node).FirstOrDefault()?.Recalculate();
        }

        protected internal override void OnInputLinked(Connection con)
        {
            if (con.To == Inputs[1])
            {
                Inputs[1].Type = con.From.Type;
                Outputs[0].Type = typeof(IEnumerable<>).MakeGenericType(Inputs[1].Type);
            }
            ForceReloop();
        }

        protected internal override void OnInputUnlinked(Connection connection)
        {
            if (connection.To == Inputs[1])
            {
                Inputs[1].Type = typeof(object);
            }
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>
                {
                    new NodeIn("Loop Link", typeof(LoopLink), false),
                    new NodeIn("Loop Item", typeof(object), false)
                },
                new List<NodeOut>
                {
                    new NodeOut("Values", typeof(IEnumerable))
                }
            );
        }
    }
}
