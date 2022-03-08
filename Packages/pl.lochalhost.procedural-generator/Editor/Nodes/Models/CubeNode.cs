using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Models
{
    [NodeName("Models/Cube")]
    internal class CubeNode : Node
    {
        public CubeNode(): base("Cube")
        {
        }

        protected override (List<NodeIn>, List<NodeOut>) SetupSockets()
        {
            return (
                new List<NodeIn>(),
                new List<NodeOut>
                {
                    new NodeOut("Model", typeof(Mesh))
                    {
                        Value = Resources.GetBuiltinResource<Mesh>("Cube.fbx")
                    }
                }
            );
        }
    }
}
