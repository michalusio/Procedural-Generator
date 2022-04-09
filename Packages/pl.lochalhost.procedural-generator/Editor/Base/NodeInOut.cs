using Packages.pl.lochalhost.procedural_generator.Runtime;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Base
{
    public abstract class NodeInOut : VisualElement, INodeInOut
    {
        public readonly ObservableCollection<Connection> Connections = new ObservableCollection<Connection>();
        public Node Node { get; private set; }
        public string Label { get; private set; }

        private Type type;
        public Type Type
        {
            get => type;
            set
            {
                type = value;
                var socket = this.Q<NodeSocket>();
                if (socket != null) socket.Type = value;
                MarkDirtyRepaint();
            }
        }

        public bool Multi { get; private set; }
        private object value;
        public object Value
        {
            get => value;
            set
            {
                if (Type.IsValueType && value == null)
                {
                    this.value = default;
                    return;
                }
                if (value != null)
                {
                    if (!Multi && !Type.IsAssignableFrom(value.GetType()))
                    {
                        throw new ApplicationException($"Value is not valid type - {value.GetType().Name} but needs {Type.Name}");
                    }
                    if (Multi)
                    {
                        if (!(value is IEnumerable list))
                        {
                            throw new ApplicationException($"Value needs a multi type but did not receive a list - {value.GetType().Name}");
                        }
                        foreach(var v in list)
                        {
                            if (v != null && !Type.IsAssignableFrom(v.GetType()))
                            {
                                throw new ApplicationException($"Element of a multi list is not valid type - {v.GetType().Name} but needs {Type.Name}");
                            }
                        }
                    }
                }
                this.value = value;
            }
        }

        protected NodeInOut(string label, Type type, bool multi = false)
        {
            Label = label;
            Multi = multi;
            Type = type;
            Connections.CollectionChanged += Connections_CollectionChanged;
        }

        public void AssignNode(Node node)
        {
            if (Node != null) throw new ApplicationException();
            Node = node;
        }

        private void Connections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            EnableInClassList("filled", Connections.Any());
        }
    }
}
