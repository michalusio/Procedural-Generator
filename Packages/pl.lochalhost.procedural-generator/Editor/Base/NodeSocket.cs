using System;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Base
{
    internal class NodeSocket: VisualElement
    {
        private Type type;
        internal Type Type
        {
            set
            {
                if (type != null)
                {
                    RemoveFromClassList(type.GetClassName());
                }
                AddToClassList(value.GetClassName());
                type = value;
                tooltip = type.PrettyName() + (Multi ? " (Multiple)" : "");
                MarkDirtyRepaint();
            }
        }

        private readonly bool Multi;

        public NodeSocket(Type type, bool multi = false)
        {
            Multi = multi;
            Type = type;
            if (Multi) {
                AddToClassList("multi");
            };
            this.AddManipulator(new SocketClicker());
            this.AddManipulator(new SocketDisconnector());
        }
    }
}
