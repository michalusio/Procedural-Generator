using NUnit.Framework;
using Packages.pl.lochalhost.procedural_generator.Editor;
using Packages.pl.lochalhost.procedural_generator.Runtime;
using UnityEditor;
using UnityEngine;

namespace lochalhost.procedural_generator.Editor.Tests
{
    [TestFixture]
    public abstract class NodeTestBase
    {
        protected ToolWindow Window { get; private set; }

        [OneTimeSetUp]
        public void BeforeSetup()
        {
            Window = ScriptableObject.CreateInstance<ToolWindow>();
            Window.rootVisualElement.visible = false;
        }

        [SetUp]
        public void Setup()
        {
            Window.asset = ScriptableObject.CreateInstance<ProceduralGeneratorAsset>();
            Window.LoadAsset();
        }

        [OneTimeTearDown]
        public void AfterTeardown()
        {
            Object.DestroyImmediate(Window.asset);
        }
    }
}
