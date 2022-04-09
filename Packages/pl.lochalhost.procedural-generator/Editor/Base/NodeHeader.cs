using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Base
{
    internal class NodeHeader : VisualElement
    {
        public NodeHeader(string label)
        {
            Add(new Label(label));
            this.AddManipulator(new Dragger());
            Add(new Button(() => (parent as Node).RemoveNode())
            {
                text = "X"
            });
        }
    }
}
