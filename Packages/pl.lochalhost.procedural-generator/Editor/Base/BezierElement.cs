using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

using ToCache = System.Collections.Generic.IEnumerable<((UnityEngine.Vector2 left, UnityEngine.Vector2 centerLeft, UnityEngine.Vector2 centerRight, UnityEngine.Vector2 right) Prev, (UnityEngine.Vector2 left, UnityEngine.Vector2 centerLeft, UnityEngine.Vector2 centerRight, UnityEngine.Vector2 right) Next)>;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Base
{
    public class BezierElement : ImmediateModeElement
    {
        private Vector2 _positionA = new Vector2(-999, -999);
        private Vector2 _positionB = new Vector2(-999, -999);

        private ToCache pointCache;

        private static Material _lineMaterial;

        public Vector2 PositionA
        {
            get { return _positionA; }
            set
            {
                var drastic = (_positionA - value).sqrMagnitude > 0.1f;
                if (drastic)
                {
                    _positionA = value;
                    UpdateBounds();
                }
            }
        }

        public Vector2 PositionB
        {
            get { return _positionB; }
            set
            {
                var drastic = (_positionB - value).sqrMagnitude > 0.1f;
                if (drastic)
                {
                    _positionB = value;
                    UpdateBounds();
                }
            }
        }

        public BezierElement()
        {
            pickingMode = PickingMode.Ignore;
            cullingEnabled = true;
            MarkDirtyRepaint();
            this.focusable = false;
        }

        void UpdateBounds()
        {
            Vector2 min = Vector2.Min(_positionA, _positionB);
            Vector2 max = Vector2.Max(_positionA, _positionB);

            style.left = min.x;
            style.top = min.y;
            style.width = max.x - min.x;
            style.height = max.y - min.y;
            pointCache = null;
        }

        protected override void ImmediateRepaint()
        {
            GetLineMaterial().SetPass(0);

            Vector2 min = Vector2.Min(_positionA, _positionB) + parent.worldBound.position;
            Vector3 relA = _positionA - min;
            Vector3 relB = _positionB - min;
            if (pointCache == null)
            {
                var points = BezierPoints(
                    relA,
                    new Vector2((relA.x + relB.x) / 2, relA.y),
                    new Vector2((relA.x + relB.x) / 2, relB.y),
                    relB);

                const int DIST = 4;
                const int NEAR = 3;

                var pointLine = points.Select((p) =>
                {
                    var normal = new Vector2(p.Item2.y, -p.Item2.x).normalized;
                    var left = p.Item1 + normal * DIST;
                    var right = p.Item1 - normal * DIST;
                    var centerLeft = p.Item1 + normal * NEAR;
                    var centerRight = p.Item1 - normal * NEAR;
                    return (left, centerLeft, centerRight, right);
                }).ToArray();

                pointCache = pointLine.Skip(1).Zip(pointLine.Take(pointLine.Length - 1), (a, b) => (Prev: b, Next: a)).ToArray();
            }

            var color1 = new Color(0.953f, 0.957f, 0.973f);
            var color2 = new Color(1, 1, 1, 0);

            GL.Begin(GL.TRIANGLES);
            foreach (var (Prev, Next) in pointCache)
            {
                GL.Color(color2);
                GL.Vertex(Prev.left);
                GL.Vertex(Next.left);
                GL.Color(color1);
                GL.Vertex(Prev.centerLeft);

                GL.Vertex(Prev.centerLeft);
                GL.Color(color2);
                GL.Vertex(Next.left);
                GL.Color(color1);
                GL.Vertex(Next.centerLeft);

                GL.Color(color2);
                GL.Vertex(Prev.right);
                GL.Vertex(Next.right);
                GL.Color(color1);
                GL.Vertex(Prev.centerRight);

                GL.Vertex(Prev.centerRight);
                GL.Color(color2);
                GL.Vertex(Next.right);
                GL.Color(color1);
                GL.Vertex(Next.centerRight);

                GL.Vertex(Prev.centerLeft);
                GL.Vertex(Next.centerLeft);
                GL.Vertex(Prev.centerRight);
                GL.Vertex(Prev.centerRight);
                GL.Vertex(Next.centerLeft);
                GL.Vertex(Next.centerRight);
            }
            GL.End();

        }

        static Material GetLineMaterial()
        {
            if (!_lineMaterial)
            {
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                _lineMaterial = new Material(shader)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                _lineMaterial.SetInt("_ZWrite", 0);
            }

            return _lineMaterial;
        }

        private static IEnumerable<(Vector2, Vector2)> BezierPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float from = 0, float to = 1)
        {
            var middleT = (from + to) / 2;

            var first = GetPoint(p0, p1, p2, p3, from);
            var last = GetPoint(p0, p1, p2, p3, to);
            
            if ((first - last).sqrMagnitude > 10*10)
            {
                foreach (var p in BezierPoints(p0, p1, p2, p3, from, middleT))
                {
                    yield return p;
                }
                foreach (var p in BezierPoints(p0, p1, p2, p3, middleT, to).Skip(1))
                {
                    yield return p;
                }
            }
            else
            {
                yield return (first, GetFirstDerivative(p0, p1, p2, p3, from));
                yield return (GetPoint(p0, p1, p2, p3, middleT), GetFirstDerivative(p0, p1, p2, p3, middleT));
                yield return (last, GetFirstDerivative(p0, p1, p2, p3, to));
            }
        }
        private static Vector2 GetPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * oneMinusT * p0 +
                3f * oneMinusT * oneMinusT * t * p1 +
                3f * oneMinusT * t * t * p2 +
                t * t * t * p3;
        }

        private static Vector2 GetFirstDerivative(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                3f * oneMinusT * oneMinusT * (p1 - p0) +
                6f * oneMinusT * t * (p2 - p1) +
                3f * t * t * (p3 - p2);
        }
    }
}
