using Packages.pl.lochalhost.procedural_generator.Runtime.Packages.pl.lochalhost.procedural_generator.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.pl.lochalhost.procedural_generator.Runtime
{
    [Serializable]
    public struct SerializableConnection
    {
        public int FromNodeIndex;
        public int FromNodeSocket;
        public int ToNodeIndex;
        public int ToNodeSocket;
    }

    [Serializable]
    public struct SerializableNode
    {
        public string NodeType;
        public float X, Y;
        public List<string> Data;
    }

    [CreateAssetMenu(fileName = "Procedural Model", menuName = "Procedural Model", order = 1)]
    [Serializable]
    public class ProceduralGeneratorAsset : ScriptableObject
    {
        public List<SerializableNode> Nodes = new List<SerializableNode>();
        public List<SerializableConnection> Connections = new List<SerializableConnection>();

        public (List<NodeInOutDescription> Inputs, List<NodeInOutDescription> Outputs) GetFree(IEnumerable<Type> nodeTypes)
        {
            var allNodes = nodeTypes
                .Select(t => t.GetConstructor(Array.Empty<Type>()).Invoke(null) as INode)
                .ToDictionary(n => n.GetType().FullName);

            var assetNodeTypes = Nodes
                .Select(n => allNodes[n.NodeType])
                .ToList();

            var freeInputs = Nodes
                .SelectMany((n, i) => assetNodeTypes[i].Inputs.Select((_, nii) => (i, nii)))
                .Except(Connections.Select(c => (c.ToNodeIndex, c.ToNodeSocket)))
                .OrderBy(x => Nodes[x.Item1].Y)
                .ToList();

            var freeOutputs = Nodes
                .SelectMany((n, i) => assetNodeTypes[i].Outputs.Select((_, nii) => (i, nii)))
                .Except(Connections.Select(c => (c.FromNodeIndex, c.FromNodeSocket)))
                .OrderBy(x => Nodes[x.Item1].Y)
                .ToList();

            return (
                freeInputs
                    .Select(x => {
                        var socket = assetNodeTypes[x.Item1].Inputs.ElementAt(x.Item2);
                        return new NodeInOutDescription(x.Item1, x.Item2, socket.Item2, socket.Item3, socket.Item1);
                    })
                    .ToList(),
                freeOutputs
                    .Select(x => {
                        var socket = assetNodeTypes[x.Item1].Outputs.ElementAt(x.Item2);
                        return new NodeInOutDescription(x.Item1, x.Item2, socket.Item2, socket.Item3, socket.Item1);
                    })
                    .ToList()
            );
        }
    }

    public struct NodeInOutDescription
    {
        public readonly int NodeIndex;
        public readonly int SocketIndex;
        public readonly Type Type;
        public readonly bool Multi;
        public readonly string Label;

        public NodeInOutDescription(int nodeIndex, int socketIndex, Type type, bool multi, string label)
        {
            NodeIndex = nodeIndex;
            SocketIndex = socketIndex;
            Type = type;
            Multi = multi;
            Label = label;
        }
    }
}