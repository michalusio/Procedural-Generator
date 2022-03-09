using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes
{
    [NodeName("Display")]
    public class DisplayNode : Node
    {
        private readonly Label label = new Label();
        public DisplayNode(): base("Display")
        {
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            Add(label);
        }

        public override void Recalculate()
        {
            if (Inputs[0].Value is IEnumerable list)
            {
                var text = new StringBuilder();
                foreach (var item in list)
                {
                    if (text.Length > 0) text.AppendLine();
                    text.Append(item?.ToString() ?? "Null");
                }
                label.text = text.ToString();
            }
            else
            {
                label.text = Inputs[0].Value?.ToString() ?? "Null";
            }
        }

        protected internal override void OnInputLinked(Connection connection)
        {
            Inputs[0].Type = Inputs[0].Connections.First().From.Type;
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
                    new NodeIn("Value", typeof(object), false)
                },
                new List<NodeOut>
                {
                }
            );
        }
    }
}
