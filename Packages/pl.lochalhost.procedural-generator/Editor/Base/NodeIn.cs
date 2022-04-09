using System;
using System.Collections;
using System.Linq;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Base
{
    public class NodeIn: NodeInOut
    {
        public NodeIn(string label, Type type, bool multi) : base(label, type, multi)
        {
            Add(new NodeSocket(type, multi));
            Add(new Label(label));
        }

        public void SetValue(object value)
        {
            if (Multi)
            {
                Value = Connections
                    .Where(c => c.From != null)
                    .SelectMany(c => c.From.Value is IEnumerable en ? en.OfType<object>() : new [] { c.From.Value })
                    .ToList()
                    .AsReadOnly();
            }
            else
            {
                Value = value;
            }
        }
    }
}
