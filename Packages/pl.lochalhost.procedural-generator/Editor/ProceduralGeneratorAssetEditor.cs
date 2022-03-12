using Packages.pl.lochalhost.procedural_generator.Editor.Nodes;
using Packages.pl.lochalhost.procedural_generator.Runtime;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Packages.pl.lochalhost.procedural_generator.Editor
{
    [CustomEditor(typeof(ProceduralGeneratorAsset))]
    public class ProceduralGeneratorAssetEditor : UnityEditor.Editor
    {
        [OnOpenAsset(99)]
        public static bool OnOpenCallback(int instanceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj is ProceduralGeneratorAsset pga)
            {
                ToolWindow.Init(pga);
                return true;
            }
            else return false;
        }

        public override void OnInspectorGUI()
        {
            var pga = target as ProceduralGeneratorAsset;
            ReadonlyField(nameof(pga.Nodes), pga.Nodes.Count.ToString());
            ReadonlyField(nameof(pga.Connections), pga.Connections.Count.ToString());

            var nodeTypes = NodeAssembly.GetRegisteredNodeTypes();

            (var inputs, var outputs) = pga.GetFree(nodeTypes);

            ReadonlyField("Free inputs", inputs.Count.ToString());

            ReadonlyField("Free outputs", outputs.Count.ToString());

            ReadonlyField("Rendering nodes", pga.Nodes.Count(n => n.NodeType == typeof(RenderNode).FullName).ToString());
        }

        private void ReadonlyField(string label, string value)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(value, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
