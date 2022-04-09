using System;
using System.Collections.Generic;

namespace Packages.pl.lochalhost.procedural_generator.Runtime
{
    public interface INodeInOut
    {
        public Type Type { get; }
        public string Label { get; }
        public bool Multi { get; }
    }

    public interface INode
    {
        public IEnumerable<INodeInOut> Inputs { get; }
        public IEnumerable<INodeInOut> Outputs { get; }
    }
}
