using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections.Generic;
using UnityEditor.UIElements;
using System.Linq;
using Packages.pl.lochalhost.procedural_generator.Runtime;
using Packages.pl.lochalhost.procedural_generator.Editor.WindowUtilities;

namespace Packages.pl.lochalhost.procedural_generator.Editor
{
    public class ToolWindow : EditorWindow
    {
        public ProceduralGeneratorAsset asset;

        private bool afterAssetLoad;

        public ToolWindow()
        {
            saveChangesMessage = "You have unsaved changes. Do you really want to close the editor and lose the changes?";
        }

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
                window.Show();
            }
        }

        public void OnEnable()
        {
            this.SetAntiAliasing(8);

            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("lochalhost-NodeStyling"));
            rootVisualElement.Add(ToolbarBuilder.Build(SaveChanges, AddNode));
            rootVisualElement.Add(new RootElement(this));

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

        public void LoadAsset()
        {
            var root = rootVisualElement.Q<RootElement>();
            root.Clear();
            Nodes.Clear();
            Connections.Clear();
            OngoingConnection = null;
            var nodeTypes = NodeAssembly.GetNodeMenuOptions().ToDictionary(n => n.Constructor.DeclaringType.FullName, n => n.Constructor);
            foreach (var node in asset.Nodes)
            {
                var newNode = nodeTypes[node.NodeType].Invoke(null) as Node;
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
            rootVisualElement.Q<RootElement>().OnDestroy();
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
                rootVisualElement.EnableInClassList("connection-from", ongoingConnection?.From != null);
                rootVisualElement.EnableInClassList("connection-to", ongoingConnection?.To != null);
            }
        }
        private Connection ongoingConnection;

        public void AddNode(Node newNode, List<string> data)
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
}
