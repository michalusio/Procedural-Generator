using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor
{
    internal class RootElement: VisualElement
    {
        public readonly ToolWindow Window;
        public RootElement(ToolWindow window)
        {
            Window = window;
            RegisterCallback<MouseUpEvent>(KillConnection);
        }

        internal void OnDestroy()
        {
            UnregisterCallback<MouseUpEvent>(KillConnection);
        }

        private void KillConnection(MouseUpEvent mue)
        {
            Window.OngoingConnection?.Remove();
            Window.OngoingConnection = null;
        }
    }
}
