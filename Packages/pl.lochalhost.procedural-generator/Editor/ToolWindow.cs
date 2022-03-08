using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections.Generic;
using UnityEditor.UIElements;
using System.Linq;
using System;
using Packages.pl.lochalhost.procedural_generator.Runtime;
using System.Reflection;
using Packages.pl.lochalhost.procedural_generator.Editor.Packages.pl.lochalhost.procedural_generator.Editor;

namespace Packages.pl.lochalhost.procedural_generator.Editor
{
    public class ToolWindow : EditorWindow
    {
        public ProceduralGeneratorAsset asset;

        private bool afterAssetLoad;

        private List<(ConstructorInfo, string)> NodeTypes;

        internal static void Init(ProceduralGeneratorAsset pga)
        {
            var assetWindow = Resources.FindObjectsOfTypeAll<ToolWindow>().FirstOrDefault(w => w.asset == pga);
            if (assetWindow != null)
            {
                assetWindow.Focus();
            }
            else
            {
                var window = CreateWindow<ToolWindow>(pga.name);
                window.asset = pga;
                window.saveChangesMessage = "You have unsaved changes. Do you really want to close the editor and lose the changes?";
                window.Show();
            }
        }

        public void OnEnable()
        {
            this.SetAntiAliasing(8);
            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("lochalhost-NodeStyling"));

            var toolbar = new Toolbar();
            rootVisualElement.Add(toolbar);

            var saveButton = new ToolbarButton(SaveChanges)
            {
                text = "Save"
            };
            toolbar.Add(saveButton);

            var nodeMenu = new ToolbarMenu
            {
                text = "Nodes"
            };
            BuildNodeMenu(nodeMenu.menu);
            toolbar.Add(nodeMenu);

            var root = new RootElement(this);
            root.RegisterCallback<MouseUpEvent>(KillConnection);
            rootVisualElement.Add(root);

            if (asset)
            {
                LoadAsset();
            }
        }

        public new void Show()
        {
            base.Show();
            LoadAsset();
        }

        internal void LoadAsset()
        {
            var root = rootVisualElement.Q<RootElement>();
            root.Clear();
            Nodes.Clear();
            Connections.Clear();
            ongoingConnection = null;
            foreach (var node in asset.Nodes)
            {
                var nodeConstructor = NodeTypes.Select(x => x.Item1).First(c => c.DeclaringType.FullName == node.NodeType);
                var newNode = nodeConstructor.Invoke(null) as Node;
                newNode.style.top = node.Y;
                newNode.style.left = node.X;
                AddNode(newNode, node.Data);
            }
            foreach (var conn in asset.Connections)
            {
                var c = Connection.MakeFrom(root, Nodes[conn.FromNodeIndex].Outputs[conn.FromNodeSocket]);
                c.LinkTo(Nodes[conn.ToNodeIndex].Inputs[conn.ToNodeSocket]);
                Connections.Add(c);
            }
            hasUnsavedChanges = false;
            afterAssetLoad = true;
        }

        public void OnGUI()
        {
            if (afterAssetLoad)
            {
                foreach(var c in Connections)
                {
                    c.Update();
                }
                afterAssetLoad = false;
            }
        }

        public void OnDestroy()
        {
            rootVisualElement[0].UnregisterCallback<MouseUpEvent>(KillConnection);
        }

        public override void SaveChanges()
        {
            asset.Nodes = Nodes.Select(n => n.Serialize()).ToList();
            asset.Connections = Connections.Select(c => c.Serialize()).ToList();
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);
            base.SaveChanges();
        }

        internal readonly List<Node> Nodes = new List<Node>();
        internal readonly List<Connection> Connections = new List<Connection>();
        internal Connection OngoingConnection
        {
            get => ongoingConnection;
            set
            {
                ongoingConnection = value;
                rootVisualElement.EnableInClassList("connection-from", ongoingConnection != null && ongoingConnection.From != null);
                rootVisualElement.EnableInClassList("connection-to", ongoingConnection != null && ongoingConnection.To != null);
            }
        }
        private Connection ongoingConnection;

        private void KillConnection(MouseUpEvent mue)
        {
            OngoingConnection?.Remove();
            OngoingConnection = null;
        }

        private void BuildNodeMenu(DropdownMenu menu)
        {
            NodeTypes = NodeAssembly.GetRegisteredNodeTypes()
                .Select(t => (NodeType: t, NodeName: t.GetCustomAttributes(typeof(NodeNameAttribute), false).FirstOrDefault() as NodeNameAttribute))
                .Where(x => x.NodeName != null)
                .Select(x => (Constructor: x.NodeType.GetConstructor(Array.Empty<Type>()), x.NodeName.Label)).ToList();
            foreach (var (Constructor, Label) in NodeTypes)
            {
                menu.AppendAction(Label, (ac) => {
                    var newNode = Constructor.Invoke(null) as Node;
                    AddNode(newNode, null);
                }, ac => DropdownMenuAction.Status.Normal);
            }
        }

        private void AddNode(Node newNode, List<string> data)
        {
            var root = rootVisualElement.Q<RootElement>();
            newNode.SetRootElement(root, data ?? new List<string>());
            root.Add(newNode);
            Nodes.Add(newNode);
        }

        internal void SetUnsavedChanges()
        {
            hasUnsavedChanges = true;
        }
    }

    internal class RootElement: VisualElement
    {
        public readonly ToolWindow Window;
        public RootElement(ToolWindow window)
        {
            Window = window;
        }
    }
}
