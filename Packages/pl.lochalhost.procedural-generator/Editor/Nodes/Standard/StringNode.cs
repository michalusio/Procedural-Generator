using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Standard
{
    [NodeName("Standard/String")]
    public class StringNode : Node
    {
        private readonly TextField field = new TextField();
        public StringNode(): base("String")
        {
            field.style.BorderWidth(1);
            field.style.BorderRadius(4);
            field.RegisterValueChangedCallback(ValueChanged);
            Add(field);
        }

        private void ValueChanged(ChangeEvent<string> evt)
        {
            Outputs[0].Value = evt.newValue;
            MarkAsChanged();
            Root.Window.SetUnsavedChanges();
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>(),
                new List<NodeOut>
                {
                    new NodeOut("Value", typeof(string))
                    {
                        Value = ""
                    }
                }
            );
        }

        protected override void LoadData(List<string> data)
        {
            if (data.Count != 1) return;
            field.value = data[0];
            Outputs[0].Value = field.value;
            MarkAsChanged();
        }

        protected override List<string> SaveData()
        {
            return new List<string>
            {
                field.value
            };
        }
    }
}
