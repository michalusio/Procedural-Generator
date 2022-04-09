using System;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Base
{
    public class NodeOut : NodeInOut
    {
        public NodeOut(string label, Type type) : base(label, type)
        {
            Add(new Label(label));
            Add(new NodeSocket(type));
        }
    }
}
