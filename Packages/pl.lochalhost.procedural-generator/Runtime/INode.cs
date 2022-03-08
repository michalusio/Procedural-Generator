using System;
using System.Collections.Generic;

namespace Packages.pl.lochalhost.procedural_generator.Runtime.Packages.pl.lochalhost.procedural_generator.Runtime
{
    public interface INode
    {
        public IEnumerable<(string, Type, bool)> Inputs { get; }
        public IEnumerable<(string, Type, bool)> Outputs { get; }
    }
}
