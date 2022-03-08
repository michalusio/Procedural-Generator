using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static Packages.pl.lochalhost.procedural_generator.Editor.Base.Node;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Base
{
    internal class Dragger : MouseManipulator
    {
        private Vector2 m_Start;
        protected bool m_Active;

        public Dragger()
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            m_Active = false;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected void OnMouseDown(MouseDownEvent e)
        {
            if (m_Active)
            {
                e.StopImmediatePropagation();
                return;
            }

            if (CanStartManipulation(e))
            {
                m_Start = e.localMousePosition;

                m_Active = true;
                target.CaptureMouse();
                target.parent.AddToClassList("dragged");
                target.parent.BringToFront();

                e.StopPropagation();
            }
        }

        protected void OnMouseMove(MouseMoveEvent e)
        {
            if (!m_Active || !target.HasMouseCapture())
                return;

            Vector2 diff = e.localMousePosition - m_Start;
            var header = target as NodeHeader;
            header.parent.style.top = Mathf.Max(header.parent.layout.y + diff.y, 0);
            header.parent.style.left = Mathf.Max(header.parent.layout.x + diff.x, 0);

            var node = header.parent as Node;
            foreach (var connection in node.Inputs.SelectMany(i => i.Connections))
            {
                connection.Update();
            }
            foreach (var connection in node.Outputs.SelectMany(i => i.Connections))
            {
                connection.Update();
            }

            e.StopPropagation();
        }

        protected void OnMouseUp(MouseUpEvent e)
        {
            if (!m_Active || !target.HasMouseCapture() || !CanStopManipulation(e))
                return;

            m_Active = false;
            target.ReleaseMouse();
            target.parent.RemoveFromClassList("dragged");
            e.StopPropagation();
        }
    }
}
