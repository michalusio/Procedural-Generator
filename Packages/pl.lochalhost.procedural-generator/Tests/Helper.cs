using NUnit.Framework;
using UnityEngine;

namespace lochalhost.procedural_generator.Editor.Tests
{
    internal static class Helper
    {
        public const float FLOAT_DELTA = 0.001f;

        public static void AssertSameMeshData(Mesh a, Mesh b)
        {
            AreEqual(a.vertices, b.vertices);
            Assert.AreEqual(a.subMeshCount, b.subMeshCount);
            Assert.AreEqual(a.triangles, b.triangles);
            AreEqual(a.normals, b.normals);
            AreEqual(a.uv, b.uv);
        }

        #region Color
        public static void AreEqual(Color expected, Color actual)
        {
            AreEqual(expected, actual, FLOAT_DELTA);
        }

        public static void AreEqual(Color expected, Color actual, float delta, string message)
        {
            if (delta <= 0)
                delta = FLOAT_DELTA;

            var expectedV = new Vector4(expected.r, expected.g, expected.b, expected.a);
            var actualV = new Vector4(actual.r, actual.g, actual.b, actual.a);

            float distance = Vector4.Distance(expectedV, actualV);

            if (string.IsNullOrEmpty(message))
                message = string.Format("Expected: Color({0}, {1}, {2}, {3})\nBut was: Color({4}, {5}, {6}, {7})\nDistance: {7} is greated than allowed delta {8}",
                                        expected.r, expected.g, expected.b, expected.a, actual.r, actual.g, actual.b, actual.a,
                                        distance, delta);

            Assert.That(distance, Is.LessThanOrEqualTo(delta), message);
        }

        public static void AreEqual(Color expected, Color actual, float delta)
        {
            AreEqual(expected, actual, delta, null);
        }

        #endregion

        #region Vector2
        public static void AreEqual(Vector2[] expected, Vector2[] actual)
        {
            AreEqual(expected, actual, FLOAT_DELTA);
        }

        public static void AreEqual(Vector2[] expected, Vector2[] actual, float delta)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                AreEqual(expected[i], actual[i], delta);
            }
        }

        public static void AreEqual(Vector2 expected, Vector2 actual, float delta, string message)
        {
            if (delta <= 0)
                delta = FLOAT_DELTA;

            float distance = Vector2.Distance(expected, actual);

            if (string.IsNullOrEmpty(message))
                message = string.Format("Expected: Vector2({0}, {1})\nBut was:  Vector2({2}, {3})\nDistance: {4} is greated than allowed delta {5}",
                                        expected.x, expected.y, actual.x, actual.y,
                                        distance, delta);

            Assert.That(distance, Is.LessThanOrEqualTo(delta), message);
        }

        public static void AreEqual(Vector2 expected, Vector2 actual, float delta)
        {
            AreEqual(expected, actual, delta, null);
        }

        public static void AreEqual(Vector2 expected, Vector2 actual, string message)
        {
            AreEqual(expected, actual, 0, message);
        }

        public static void AreEqual(Vector2 expected, Vector2 actual)
        {
            AreEqual(expected, actual, 0, null);
        }
        #endregion

        #region Vector3
        public static void AreEqual(Vector3[] expected, Vector3[] actual)
        {
            AreEqual(expected, actual, FLOAT_DELTA);
        }

        public static void AreEqual(Vector3[] expected, Vector3[] actual, float delta)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                AreEqual(expected[i], actual[i], delta);
            }
        }

        public static void AreEqual(Vector3 expected, Vector3 actual, float delta, string message)
        {
            if (delta <= 0)
                delta = FLOAT_DELTA;

            float distance = Vector3.Distance(expected, actual);

            if (string.IsNullOrEmpty(message))
                message = string.Format("Expected: Vector3({0}, {1}, {2})\nBut was:  Vector3({3}, {4}, {5})\nDistance: {6} is greated than allowed delta {7}",
                                        expected.x, expected.y, expected.z,
                                        actual.x, actual.y, actual.z,
                                        distance, delta);

            Assert.That(distance, Is.LessThanOrEqualTo(delta), message);
        }

        public static void AreEqual(Vector3 expected, Vector3 actual, float delta)
        {
            AreEqual(expected, actual, delta, null);
        }

        public static void AreEqual(Vector3 expected, Vector3 actual, string message)
        {
            AreEqual(expected, actual, 0, message);
        }

        public static void AreEqual(Vector3 expected, Vector3 actual)
        {
            AreEqual(expected, actual, 0, null);
        }
        #endregion
    }
}
