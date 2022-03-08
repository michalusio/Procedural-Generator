using System.Linq;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Base
{
    internal class SocketDisconnector : MouseManipulator
    {
        public SocketDisconnector()
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.RightMouse });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected void OnMouseUp(MouseUpEvent e)
        {
            if (e.button != (int)MouseButton.RightMouse) return;
            var nodeInOut = target.parent as NodeInOut;
            foreach(var con in nodeInOut.Connections.ToArray())
            {
                con.Remove();
            }
            nodeInOut.GetFirstAncestorOfType<RootElement>().Window.SetUnsavedChanges();
        }
    }
}
