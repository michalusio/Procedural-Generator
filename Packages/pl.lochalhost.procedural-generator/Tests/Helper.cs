using NUnit.Framework;
using UnityEngine;

namespace lochalhost.procedural_generator.Editor.Tests
{
    internal static class Helper
    {
        public static void AssertSameMeshData(Mesh a, Mesh b)
        {
            Assert.AreEqual(a.vertexCount, b.vertexCount);
            Assert.AreEqual(a.vertices, b.vertices);
            Assert.AreEqual(a.subMeshCount, b.subMeshCount);
            Assert.AreEqual(a.triangles, b.triangles);
            Assert.AreEqual(a.normals, b.normals);
            Assert.AreEqual(a.uv, b.uv);
        }
    }
}
