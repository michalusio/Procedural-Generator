using System.Collections.Generic;
using NUnit.Framework;
using Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Math;

namespace lochalhost.procedural_generator.Editor.Tests
{
    public class ArithmeticNodeTests : NodeTestBase
    {
        [Test]
        public void ShouldAddTwoNumbers()
        {
            Assert.IsNotNull(Window);
            var node = new ArithmeticNode();
            Window.AddNode(node, new List<string> { "Addition" });
            node.Inputs[0].Value = 4f;
            node.Inputs[1].Value = new List<object> { 7f };
            node.Recalculate();
            Assert.AreEqual(11f, node.Outputs[0].Value);
        }
    }
}
