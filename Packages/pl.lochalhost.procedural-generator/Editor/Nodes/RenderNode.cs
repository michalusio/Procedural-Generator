using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes
{
    [NodeName("Render")]
    public class RenderNode : Node
    {
        private readonly MeshDisplay display = new MeshDisplay();

        public RenderNode(): base("Render")
        {
            Add(display);
            var button = new Button(SaveModel)
            {
                text = "Save model"
            };
            Add(button);
        }

        public override void Recalculate()
        {
            var mesh = Inputs[0].Value as Mesh;
            if (mesh != null)
            {
                display.Mesh = mesh;
            }
        }

        private void SaveModel()
        {
            var meshPath = GetDerivedAssetPath();
            AssetDatabase.DeleteAsset(meshPath);
            Mesh meshToSave = Object.Instantiate(display.Mesh);
            MeshUtility.Optimize(meshToSave);
            MeshUtility.SetMeshCompression(meshToSave, ModelImporterMeshCompression.Low);

            AssetDatabase.CreateAsset(meshToSave, meshPath);
            AssetDatabase.SaveAssets();
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>
                {
                    new NodeIn("Model", typeof(Mesh), false)
                },
                new List<NodeOut>
                {
                }
            );
        }
    }
}
