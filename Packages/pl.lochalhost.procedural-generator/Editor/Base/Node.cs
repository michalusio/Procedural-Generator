using Packages.pl.lochalhost.procedural_generator.Runtime;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Base
{
    public abstract partial class Node: VisualElement, ISerializable<SerializableNode>, INode
    {
        public IList<NodeIn> Inputs => InNodes.Nodes;
        public IList<NodeOut> Outputs => OutNodes.Nodes;
        IEnumerable<INodeInOut> INode.Inputs => Inputs;
        IEnumerable<INodeInOut> INode.Outputs => Outputs;

        private readonly NodeGroup<NodeIn> InNodes;
        private readonly NodeGroup<NodeOut> OutNodes;

        private RootElement Root { get; set; }

        protected Node(string label)
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
            RecalculateMinHeight();
        }

        internal void SetRootElement(RootElement root, List<string> data)
        {
            Root = root;
            SetUnsavedChanges();
            LoadData(data);
        }

        /// <summary>
        /// Called when a new connection is made to an input of the node
        /// </summary>
        /// <param name="connection">The connection that was made</param>
        protected internal virtual void OnInputLinked(Connection connection) { }

        /// <summary>
        /// Called when a connection is unlinked from an input of the node
        /// </summary>
        /// <param name="connection">The connection that was removed</param>
        protected internal virtual void OnInputUnlinked(Connection connection) { }

        /// <summary>
        /// Called when a new connection is made to an output of the node
        /// </summary>
        /// <param name="connection">The connection that was made</param>
        protected internal virtual void OnOutputLinked(Connection connection) { }

        /// <summary>
        /// Called when a connection is unlinked from an output of the node
        /// </summary>
        /// <param name="connection">The connection that was removed</param>
        protected internal virtual void OnOutputUnlinked(Connection connection) { }

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
            RecalculateMinHeight();
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

        /// <summary>
        /// Marks the window the node resides in as having changes that will need to be saved.
        /// </summary>
        protected void SetUnsavedChanges()
        {
            Root.Window.SetUnsavedChanges();
        }

        /// <summary>
        /// Returns a path for the derived asset (could be used to save a result of operations of a node)
        /// </summary>
        /// <returns>A path for an asset derived from the node</returns>
        protected string GetDerivedAssetPath()
        {
            var assetPath = AssetDatabase.GetAssetPath(Root.Window.asset);
            var directoryPath = Path.GetDirectoryName(assetPath);
            var indexOfThisNode = Root.Window.Nodes.TakeWhile(n => n != this).Count();
            return Path.Combine(directoryPath, $"{Root.Window.asset.name}_{indexOfThisNode}.asset");
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

        internal void RemoveNode()
        {
            RemoveFromHierarchy();
            InNodes.Clear();
            OutNodes.Clear();
            Root.Window.Nodes.Remove(this);
            Root.Window.SetUnsavedChanges();
        }

        private void RecalculateMinHeight()
        {
            style.minHeight = 24 + Mathf.Max(InNodes.Nodes.Count, OutNodes.Nodes.Count) * 14;
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
}
