using UnityEngine;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Base
{
    public class MeshDisplay : ImmediateModeElement
    {
        private RenderTexture _texture;
        private static Material _meshMaterial;
        private static Material _wiremeshMaterial;

        private Mesh _mesh;
        public Mesh Mesh
        {
            get { return _mesh; }
            set
            {
                if (_mesh != value)
                {
                    _mesh = value;
                    MarkDirtyRepaint();
                }
            }
        }

        public MeshDisplay()
        {
            pickingMode = PickingMode.Ignore;
        }

        protected override void ImmediateRepaint()
        {
            if (Mesh != null)
            {
                GL.PushMatrix();
                var texture = GetTexture();
                var previousTargetColor = Graphics.activeColorBuffer;
                var previousTargetDepth = Graphics.activeDepthBuffer;
                Graphics.SetRenderTarget(texture);
                GL.Clear(true, true, Color.clear);

                GL.LoadProjectionMatrix(Matrix4x4.Perspective(60, texture.width / (float) texture.height, 0.5f, 100));
                GL.modelview = GetViewMatrix();

                GetMeshMaterial().SetPass(0);
                Graphics.DrawMeshNow(Mesh, Matrix4x4.identity);

                GetWireframeMeshMaterial().SetPass(0);
                Graphics.DrawMeshNow(Mesh, Matrix4x4.identity);
                GL.PopMatrix();

                Graphics.SetRenderTarget(previousTargetColor, previousTargetDepth);
                Graphics.DrawTexture(new Rect(Vector2.one * 2, Vector2.one * 256), texture);
            }
        }

        private RenderTexture GetTexture()
        {
            if (!_texture)
            {
                _texture = new RenderTexture((int)localBound.width, (int)localBound.height, 16, RenderTextureFormat.ARGB32);
                _texture.Create();
            }
            
            return _texture;
        }

        private static Matrix4x4 GetViewMatrix()
        {
            var view = Matrix4x4.LookAt(new Vector3(2, 2, 2.5f), Vector3.zero, Vector3.up).inverse;
            if (SystemInfo.usesReversedZBuffer)
            {
                view.m20 = -view.m20;
                view.m21 = -view.m21;
                view.m22 = -view.m22;
                view.m23 = -view.m23;
            }

            return view;
        }

        static Material GetWireframeMeshMaterial()
        {
            if (!_wiremeshMaterial)
            {
                _wiremeshMaterial = Resources.Load("lochalhost-Wireframe-Solid", typeof(Material)) as Material;
            }

            return _wiremeshMaterial;
        }

        static Material GetMeshMaterial()
        {
            if (!_meshMaterial)
            {
                _meshMaterial = Resources.Load("lochalhost-Solid", typeof(Material)) as Material;
            }

            return _meshMaterial;
        }
    }
}
