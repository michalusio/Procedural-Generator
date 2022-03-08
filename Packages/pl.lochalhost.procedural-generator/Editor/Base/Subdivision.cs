using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Base
{
    internal static class Subdivision
    {
        private struct Edge
        {
            public int A;
            public int B;

            public Edge(int a, int b)
            {
                A = a;
                B = b;
            }

            public override bool Equals(object obj)
            {
                return obj is Edge y && Math.Min(A, B) == Math.Min(y.A, y.B) && Math.Max(A, B) == Math.Max(y.A, y.B);
            }

            public override int GetHashCode()
            {
                return Math.Min(A, B) ^ (Math.Max(A, B) << 16);
            }
        }

        internal static Mesh Simple(Mesh mesh)
        {
            var oldVertices = mesh.vertices;
            var oldNormals = mesh.normals;
            var oldTriangles = mesh.triangles.GroupInto(3).ToList();
            var edges = oldTriangles
                        .SelectMany(t => GetEdges(t))
                        .Distinct()
                        .ToList();

            var newEdgeData = edges.Select((e, i) => (e, i)).ToDictionary(edge => edge.e, edge => (
                Index: edge.i + oldVertices.Length,
                Vertex: (oldVertices[edge.e.A] + oldVertices[edge.e.B]) / 2,
                Normal: (oldNormals[edge.e.A] + oldNormals[edge.e.B]) / 2
            ));

            return new Mesh
            {
                vertices = oldVertices.Concat(newEdgeData.Select(e => e.Value.Vertex)).ToArray(),
                normals = oldNormals.Concat(newEdgeData.Select(e => e.Value.Normal)).ToArray(),
                triangles = oldTriangles.SelectMany(t => SubdivideTriangle(t, newEdgeData)).ToArray()
            };
        }

        private static IEnumerable<int> SubdivideTriangle(int[] array, Dictionary<Edge, (int Index, Vector3 Vertex, Vector3 Normal)> newEdgeData)
        {
            var dataA = newEdgeData[new Edge(array[0], array[1])].Index;
            var dataB = newEdgeData[new Edge(array[1], array[2])].Index;
            var dataC = newEdgeData[new Edge(array[0], array[2])].Index;

            yield return array[0]; yield return dataA; yield return dataC;
            yield return dataA; yield return array[1]; yield return dataB;
            yield return dataA; yield return dataB; yield return dataC;
            yield return dataC; yield return dataB; yield return array[2];

        }

        private static IEnumerable<Edge> GetEdges(IEnumerable<int> triangle)
        {
            var array = triangle.ToArray();
            yield return new Edge(array[0], array[1]);
            yield return new Edge(array[1], array[2]);
            yield return new Edge(array[0], array[2]);
        }
    }
}