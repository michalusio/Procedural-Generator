using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Models
{
    [NodeName("Models/Plane")]
    public class PlaneNode : Node
    {
        public PlaneNode(): base("Plane")
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
                        Value = Resources.GetBuiltinResource<Mesh>("Plane.fbx")
                    }
                }
            );
        }
    }
}
