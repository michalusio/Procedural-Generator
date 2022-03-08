using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Math
{
    [NodeName("Math/Round")]
    internal class RoundNode : Node
    {
        private enum RoundingMode
        {
            Floor,
            Round,
            Ceil
        }

        private readonly PopupField<RoundingMode> dropdown = new PopupField<RoundingMode>(
            "Mode",
            Extensions.Values<RoundingMode>(),
            RoundingMode.Round
        );

        public RoundNode(): base("Round")
        {
            Add(dropdown);
            dropdown.RegisterCallback<ChangeEvent<RoundingMode>>(evt => Recalculate());
        }

        public override void Recalculate()
        {
            var value = (Inputs[0].Value as float?) ?? 0;
            switch (dropdown.value)
            {
                case RoundingMode.Floor:
                    Outputs[0].Value = Mathf.FloorToInt(value);
                    break;
                case RoundingMode.Round:
                    Outputs[0].Value = Mathf.RoundToInt(value);
                    break;
                case RoundingMode.Ceil:
                    Outputs[0].Value = Mathf.CeilToInt(value);
                    break;
            }
            MarkAsChanged();
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>()
                {
                    new NodeIn("In", typeof(float), false)
                },
                new List<NodeOut>
                {
                    new NodeOut("Out", typeof(int))
                    {
                        Value = 0
                    }
                }
            );
        }

        protected override void LoadData(List<string> data)
        {
            if (data.Count != 1) return;
            dropdown.value = (RoundingMode)Enum.Parse(typeof(RoundingMode), data[0]);
        }

        protected override List<string> SaveData()
        {
            return new List<string>
            {
                dropdown.value.ToString()
            };
        }
    }
}
