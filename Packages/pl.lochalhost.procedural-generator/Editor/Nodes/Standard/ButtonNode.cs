using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Standard
{
    [NodeName("Standard/Button")]
    public class ButtonNode : Node
    {
        private readonly Button btn;
        public ButtonNode(): base("Button")
        {
            btn = new Button(Recalculate)
            {
                text = "Click me"
            };
            Add(btn);
        }

        public override void Recalculate()
        {
            Outputs[0].Value = true;
            MarkAsChanged();
            Outputs[0].Value = false;
            MarkAsChanged();
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>(),
                new List<NodeOut>
                {
                    new NodeOut("Signal", typeof(object))
                    {
                        Value = false
                    }
                }
            );
        }
    }
}
