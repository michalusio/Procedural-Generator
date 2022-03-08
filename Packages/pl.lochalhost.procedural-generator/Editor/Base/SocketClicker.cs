using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Base
{
    internal class SocketClicker : MouseManipulator
    {
        public SocketClicker()
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected void OnMouseDown(MouseDownEvent e)
        {
            if (e.button != (int)MouseButton.LeftMouse) return;
            var root = target.GetFirstAncestorOfType<RootElement>();
            var nodeInOut = target.parent as NodeInOut;

            if (root.Window.OngoingConnection != null) return;

            if (nodeInOut is NodeIn ni)
            {
                root.Window.OngoingConnection = Connection.MakeTo(root, ni);
                root.Window.Connections.Add(root.Window.OngoingConnection);
                e.StopPropagation();
            }
            if (nodeInOut is NodeOut no)
            {
                root.Window.OngoingConnection = Connection.MakeFrom(root, no);
                root.Window.Connections.Add(root.Window.OngoingConnection);
                e.StopPropagation();
            }
        }

        protected void OnMouseUp(MouseUpEvent e)
        {
            if (e.button != (int)MouseButton.LeftMouse) return;
            var window = target.GetFirstAncestorOfType<RootElement>().Window;
            var connection = window.OngoingConnection;
            var nodeInOut = target.parent;

            if (connection == null) return;

            if (connection.To == null
             && nodeInOut is NodeIn ni
             && (ni.Connections.Count == 0 || (ni.Multi && ni.Connections.All(c => c.From != connection.From || c.To != ni)))
             && (ni.Type.IsAssignableFrom(connection.From.Type) || (ni.Multi && typeof(IEnumerable<>).MakeGenericType(ni.Type).IsAssignableFrom(connection.From.Type)))
             && ni.Node != connection.From.Node)
            {
                connection.LinkTo(ni);
                window.OngoingConnection = null;
                e.StopPropagation();
            }
            if (connection.From == null
             && nodeInOut is NodeOut no
             && (connection.To.Type.IsAssignableFrom(no.Type) || (connection.To.Multi && typeof(IEnumerable<>).MakeGenericType(connection.To.Type).IsAssignableFrom(no.Type)))
             && no.Node != connection.To.Node)
            {
                connection.LinkFrom(no);
                window.OngoingConnection = null;
                e.StopPropagation();
            }
        }
    }
}
