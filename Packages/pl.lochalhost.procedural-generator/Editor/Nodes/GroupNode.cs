using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using Packages.pl.lochalhost.procedural_generator.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes
{
    [NodeName("Group")]
    public class GroupNode : Node
    {
        private readonly ObjectField objectField = new ObjectField
        {
            objectType = typeof(ProceduralGeneratorAsset),
            allowSceneObjects = false
        };
        public GroupNode(): base("Group")
        {
            Add(objectField);
            objectField.RegisterCallback<ChangeEvent<Object>>(ChangeInternalNode);
        }

        private void ChangeInternalNode(ChangeEvent<Object> evt)
        {
            ResetSockets();
            this.Q<ObjectField>().Q<Label>().text = objectField.value?.name ?? "None";
            this.Q<NodeHeader>().Q<Label>().text = objectField.value?.name ?? "Group";
            Root.Window.SetUnsavedChanges();
        }

        public override void Recalculate()
        {
            var asset = objectField.value as ProceduralGeneratorAsset;
            var nodeTypes = NodeAssembly.GetRegisteredNodeTypes();

            var t = ScriptableObject.CreateInstance<ToolWindow>();
            Debug.Log("Group called: " + asset.name);

            t.asset = asset;
            t.rootVisualElement.visible = false;
            t.LoadAsset();
            (var inputs, var outputs) = asset.GetFree(nodeTypes);
            Debug.Log($"Setting {inputs.Count} inputs");
            for (int i = 0; i < inputs.Count; i++)
            {
                NodeInOutDescription input = inputs[i];
                t.Nodes[input.NodeIndex].Inputs[input.SocketIndex].Value = Inputs[i].Value;
            }
            var nodes = inputs.Select(i => t.Nodes[i.NodeIndex]).Distinct().ToList();
            Debug.Log($"Recalculating {nodes.Count} nodes");
            foreach (var node in nodes)
            {
                node.Recalculate();
            }
            Debug.Log($"Getting {outputs.Count} outputs");
            for (int i = 0; i < outputs.Count; i++)
            {
                NodeInOutDescription output = outputs[i];
                Outputs[i].Value = t.Nodes[output.NodeIndex].Outputs[output.SocketIndex].Value;
            }
            Debug.Log("Group done");
            t.asset = null;
            MarkAsChanged();
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            if (!objectField.value)
            {
                return (
                    new List<NodeIn>(),
                    new List<NodeOut>()
                );
            }
            var asset = objectField.value as ProceduralGeneratorAsset;
            var nodeTypes = NodeAssembly.GetRegisteredNodeTypes();

            (var inputs, var outputs) = asset.GetFree(nodeTypes);

            return (
                inputs.Select(i => new NodeIn(i.Label, i.Type, i.Multi)).ToList(),
                outputs.Select(i => new NodeOut(i.Label, i.Type)).ToList()
            );
        }

        protected override void LoadData(List<string> data)
        {
            if (data.Count != 1) return;
            objectField.value = AssetDatabase.LoadAssetAtPath<ProceduralGeneratorAsset>(AssetDatabase.GUIDToAssetPath((string)data[0]));
            this.Q<NodeHeader>().Q<Label>().text = objectField.value?.name ?? "Group";
            ResetSockets();
        }

        protected override List<string> SaveData()
        {
            return new List<string>
            {
                AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(objectField.value as ProceduralGeneratorAsset))
            };
        }
    }
}
