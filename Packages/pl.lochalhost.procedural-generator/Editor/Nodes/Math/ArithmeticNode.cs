using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Math
{
    [NodeName("Math/Arithmetic")]
    public class ArithmeticNode : Node
    {
        public enum Operation
        {
            Addition,
            Subtraction,
            Multiplication,
            Division,
            SquareRoot,
            Sin,
            Cos,
            Tan,
            Atan
        }

        private readonly PopupField<Operation> dropdown = new PopupField<Operation>(
            "Operation",
            Extensions.Values<Operation>(),
            Operation.Addition
        );

        public ArithmeticNode(): base("Arithmetic")
        {
            Add(dropdown);
            dropdown.RegisterCallback<ChangeEvent<Operation>>(evt => Recalculate());
        }

        public override void Recalculate()
        {
            var newValue = ((Inputs[1].Value as IEnumerable) ?? new List<object>())
                            .OfType<float>()
                            .Aggregate(FirstDo((Inputs[0].Value as float?) ?? 0f), Do);
            if (newValue != (float)Outputs[0].Value)
            {
                Outputs[0].Value = newValue;
                MarkAsChanged();
            }
        }

        private float FirstDo(float v)
        {
            switch (dropdown.value)
            {
                default:
                    return v;
                case Operation.SquareRoot:
                    return Mathf.Sqrt(v);
                case Operation.Sin:
                    return Mathf.Sin(v);
                case Operation.Cos:
                    return Mathf.Cos(v);
                case Operation.Tan:
                    return Mathf.Tan(v);
            }
        }

        private float Do(float result, float value)
        {
            switch (dropdown.value)
            {
                case Operation.Addition:
                    return result + value;
                case Operation.Subtraction:
                    return result - value;
                case Operation.Multiplication:
                    return result * value;
                case Operation.Division:
                    return result / value;
                case Operation.Atan:
                    return Mathf.Atan2(result, value);
                default:
                    return result;
            }
        }

        protected override void LoadData(List<string> data)
        {
            if (data.Count != 1) return;
            dropdown.value = (Operation)Enum.Parse(typeof(Operation), data[0]);
        }

        protected override List<string> SaveData()
        {
            return new List<string>
            {
                dropdown.value.ToString()
            };
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>()
                {
                    new NodeIn("Value", typeof(float), false),
                    new NodeIn("Values", typeof(float), true)
                },
                new List<NodeOut>
                {
                    new NodeOut("Result", typeof(float))
                    {
                        Value = 0f
                    }
                }
            );
        }
    }
}
