using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Standard
{
    [NodeName("Standard/Integer/Basic")]
    public class IntegerNode : Node
    {
        private readonly IntegerField field = new IntegerField();

        public IntegerNode(): base("Integer")
        {
            field.RegisterValueChangedCallback(ValueChanged);
            Add(field);
        }

        private void ValueChanged(ChangeEvent<int> evt)
        {
            Outputs[0].Value = evt.newValue;
            MarkAsChanged();
            SetUnsavedChanges();
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
