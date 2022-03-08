using Packages.pl.lochalhost.procedural_generator.Editor.Packages.pl.lochalhost.procedural_generator.Editor.Base;
using Packages.pl.lochalhost.procedural_generator.Runtime;
using Packages.pl.lochalhost.procedural_generator.Runtime.Packages.pl.lochalhost.procedural_generator.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Base
{
    internal abstract class Node: VisualElement, ISerializable<Node, SerializableNode>, INode
    {
        public IList<NodeIn> Inputs => InNodes.Nodes;
        public IList<NodeOut> Outputs => OutNodes.Nodes;

        IEnumerable<(string, Type, bool)> INode.Inputs => Inputs.Select(i => (i.Label, i.Type, i.Multi));

        IEnumerable<(string, Type, bool)> INode.Outputs => Outputs.Select(i => (i.Label, i.Type, i.Multi));

        private readonly NodeGroup<NodeIn> InNodes;
        private readonly NodeGroup<NodeOut> OutNodes;

        protected RootElement Root { get; private set; }

        public Node(string label)
        {
            InNodes = new NodeGroup<NodeIn>(this);
            OutNodes = new NodeGroup<NodeOut>(this);
            AddToClassList("node");
            Add(new NodeHeader(label));
            var body = new VisualElement();
            body.Add(InNodes);
            body.Add(OutNodes);
            Add(body);
            InNodes.AddToClassList("in");
            OutNodes.AddToClassList("out");
            (var inNodes, var outNodes) = SetupSockets();
            foreach (var node in inNodes) {
                InNodes.AddNode(node);
            }
            foreach (var node in outNodes)
            {
                OutNodes.AddNode(node);
            }
            style.minHeight = 24 + Mathf.Max(InNodes.Nodes.Count, OutNodes.Nodes.Count) * 14;
        }

        internal void SetRootElement(RootElement root, List<string> data)
        {
            this.Root = root;
            this.Root.Window.SetUnsavedChanges();
            LoadData(data);
        }

        protected internal virtual void OnInputLinked(Connection connection) { }
        protected internal virtual void OnInputUnlinked(Connection connection) { }

        protected internal virtual void OnOutputLinked(Connection connection) { }
        protected internal virtual void OnOutputUnlinked(Connection connection) { }

        private void RemoveNode()
        {
            RemoveFromHierarchy();
            InNodes.Clear();
            OutNodes.Clear();
            Root.Window.Nodes.Remove(this);
            Root.Window.SetUnsavedChanges();
        }

        /// <summary>
        /// This method should provide all the inputs and outputs that should be shown for the node
        /// </summary>
        /// <returns>The declaration of all inputs and outputs</returns>
        protected abstract (List<NodeIn>, List<NodeOut>) SetupSockets();
        /// <summary>
        /// Called when the node's input have been changed. Use it to recalculate the outputs, then call <see cref="MarkAsChanged">MarkAsChanged()</see>
        /// </summary>
        public virtual void Recalculate() { }
        /// <summary>
        /// Called when the node is loaded, allowing to load custom data like control values
        /// </summary>
        /// <param name="data">The data saved for this node</param>
        protected virtual void LoadData(List<string> data) { }
        /// <summary>
        /// Called when the node is to be saved, allowing to save custom data like control values
        /// </summary>
        /// <returns>The data that should be saved for this node</returns>
        protected virtual List<string> SaveData() { return new List<string>(); }
        /// <summary>
        /// Clears the inputs and outputs of the node and calls <see cref="SetupSockets">SetupSockets()</see> again
        /// </summary>
        protected void ResetSockets()
        {
            InNodes.Clear();
            OutNodes.Clear();
            (var inNodes, var outNodes) = SetupSockets();
            foreach (var node in inNodes)
            {
                InNodes.AddNode(node);
            }
            foreach (var node in outNodes)
            {
                OutNodes.AddNode(node);
            }
            style.minHeight = 24 + Mathf.Max(InNodes.Nodes.Count, OutNodes.Nodes.Count) * 14;
        }
        /// <summary>
        /// Marks the node as having changed the outputs. Passes the changes to the nodes connected to the outputs.
        /// </summary>
        protected void MarkAsChanged()
        {
            foreach ((var input, var value) in Outputs.SelectMany(
                o => o.Connections.Where(c => c.To != null).Select(c => (input: c.To, value: o.Value))
            ))
            {
                input.SetValue(value);
            }
            var allNodes = Outputs.SelectMany(o => o.Connections.Where(c => c.To != null).Select(c => c.To.Node)).ToList();
            var alreadyDone = new HashSet<Node>();
            foreach (var node in allNodes)
            {
                if (alreadyDone.Add(node)) node.Recalculate();
            }
        }

        public SerializableNode Serialize()
        {
            return new SerializableNode
            {
                NodeType = GetType().FullName,
                X = style.left.value.value,
                Y = style.top.value.value,
                Data = SaveData()
            };
        }

        internal class NodeHeader : VisualElement
        {
            public NodeHeader(string label)
            {
                Add(new Label(label));
                this.AddManipulator(new Dragger());
                var exit = new Button(() => (parent as Node).RemoveNode())
                {
                    text = "X"
                };
                Add(exit);
            }
        }

        private class NodeGroup<T> : VisualElement where T : NodeInOut
        {
            public readonly Node Node;
            public IList<T> Nodes => nodes.AsReadOnly();
            private readonly List<T> nodes = new List<T>();

            public NodeGroup(Node node)
            {
                Node = node;
            }

            public void AddNode(T node)
            {
                node.AssignNode(Node);
                Add(node);
                nodes.Add(node);
            }

            public new void Clear()
            {
                base.Clear();
                foreach (var connection in nodes.SelectMany(i => i.Connections).ToArray())
                {
                    connection.Remove();
                }
                nodes.Clear();
            }
        }
    }

    internal class NodeIn: NodeInOut
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

    internal class NodeOut : NodeInOut
    {
        public NodeOut(string label, Type type) : base(label, type)
        {
            Add(new Label(label));
            Add(new NodeSocket(type));
        }
    }

    internal class NodeInOut : VisualElement
    {
        public readonly ObservableCollection<Connection> Connections = new ObservableCollection<Connection>();
        public Node Node { get; private set; }
        public readonly string Label;

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

        public readonly bool Multi;
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

        public NodeInOut(string label, Type type, bool multi = false)
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
