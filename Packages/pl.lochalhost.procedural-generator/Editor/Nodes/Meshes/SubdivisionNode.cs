using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Meshes
{
    [NodeName("Meshes/Subdivision")]
    public class SubdivisionNode : Node
    {
        public enum SubdivisionType
        {
            Simple
        }

        private readonly PopupField<SubdivisionType> dropdown = new PopupField<SubdivisionType>(
            "Subdivision Type",
            Extensions.Values<SubdivisionType>(),
            SubdivisionType.Simple
        );
        private readonly IntegerField field = new IntegerField("Iterations")
        {
            value = 1
        };

        public SubdivisionNode(): base("Subdivision")
        {
            Add(dropdown);
            Add(field);
            field.RegisterValueChangedCallback(IterationValueChanged);
            dropdown.RegisterCallback<ChangeEvent<SubdivisionType>>(evt => Recalculate());
        }

        private void IterationValueChanged(ChangeEvent<int> evt)
        {
            var value = evt.newValue;
            if (value >= 0 && value < 6)
            {
                Recalculate();
                SetUnsavedChanges();
            }
            else field.value = evt.previousValue;
        }

        public override void Recalculate()
        {
            if (Inputs[0].Value is Mesh mesh)
            {
                Mesh newMesh;
                for (int i = 0; i < field.value; i++)
                {
                    switch (dropdown.value)
                    {
                        case SubdivisionType.Simple:
                            newMesh = Subdivision.Simple(mesh);
                            break;
                        default:
                            throw new ApplicationException();
                    }
                    mesh = newMesh;
                }
                Outputs[0].Value = mesh;
                MarkAsChanged();
            }
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>()
                {
                    new NodeIn("Model", typeof(Mesh), false)
                },
                new List<NodeOut>
                {
                    new NodeOut("Result", typeof(Mesh))
                }
            );
        }

        protected override void LoadData(List<string> data)
        {
            if (data.Count != 2) return;
            field.value = int.Parse(data[0]);
            dropdown.value = (SubdivisionType)Enum.Parse(typeof(SubdivisionType), data[1]);
        }

        protected override List<string> SaveData()
        {
            return new List<string>
            {
                field.value.ToString(),
                dropdown.value.ToString()
            };
        }
    }
}
