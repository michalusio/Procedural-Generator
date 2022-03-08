using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Standard
{
    [NodeName("Standard/Integer/0-360")]
    internal class Integer0_360Node : Node
    {
        private readonly Label label = new Label("0");
        private readonly SliderInt field = new SliderInt
        {
            highValue = 360,
            lowValue = 0
        };

        public Integer0_360Node(): base("0 - 360")
        {
            field.RegisterValueChangedCallback(ValueChanged);
            Add(label);
            Add(field);
        }

        private void ValueChanged(ChangeEvent<int> evt)
        {
            Outputs[0].Value = evt.newValue;
            label.text = evt.newValue.ToString();
            MarkDirtyRepaint();
            MarkAsChanged();
            Root.Window.SetUnsavedChanges();
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>(),
                new List<NodeOut>
                {
                    new NodeOut("Value", typeof(int))
                    {
                        Value = 0
                    }
                }
            );
        }

        protected override void LoadData(List<string> data)
        {
            if (data.Count != 1) return;
            field.value = int.Parse(data[0]);
            Outputs[0].Value = field.value;
            MarkAsChanged();
        }

        protected override List<string> SaveData()
        {
            return new List<string>
            {
                field.value.ToString()
            };
        }
    }
}
