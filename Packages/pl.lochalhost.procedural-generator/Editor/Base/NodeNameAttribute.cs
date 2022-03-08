using System;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Base
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class NodeNameAttribute : Attribute
    {
        public readonly string Label;
        public NodeNameAttribute(string label)
        {
            Label = label;
        }
    }
}
