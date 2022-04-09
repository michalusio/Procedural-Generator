using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.WindowUtilities
{
    internal static class ToolbarBuilder
    {
        public static Toolbar Build(Action saveEvent, Action<Node, List<string>> addNode)
        {
            var toolbar = new Toolbar();
            var saveButton = new ToolbarButton(saveEvent)
            {
                text = "Save"
            };
            toolbar.Add(saveButton);

            var nodeMenu = new ToolbarMenu
            {
                text = "Nodes"
            };
            BuildNodeMenu(nodeMenu.menu, addNode);
            toolbar.Add(nodeMenu);
            return toolbar;
        }

        private static void BuildNodeMenu(DropdownMenu menu, Action<Node, List<string>> addNode)
        {
            foreach (var (Constructor, Label) in NodeAssembly.GetNodeMenuOptions())
            {
                menu.AppendAction(Label, (ac) => {
                    var newNode = Constructor.Invoke(null) as Node;
                    addNode(newNode, null);
                }, ac => DropdownMenuAction.Status.Normal);
            }
        }
    }
}
