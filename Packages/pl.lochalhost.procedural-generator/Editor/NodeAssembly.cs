using Packages.pl.lochalhost.procedural_generator.Editor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Packages.pl.lochalhost.procedural_generator.Editor
{
    public static class NodeAssembly
    {
        private static readonly HashSet<Type> nodeTypes = new HashSet<Type>();

        static NodeAssembly()
        {
            RegisterNodeTypes(typeof(Node).Assembly);
        }

        internal static IEnumerable<Type> GetRegisteredNodeTypes()
        {
            return nodeTypes.AsEnumerable();
        }

        public static void RegisterNodeTypes(params Type[] types)
        {
            foreach (var t in types.Where(t => t.IsSubclassOf(typeof(Node))))
            {
                nodeTypes.Add(t);
            }
        }

        public static void RegisterNodeTypes(IEnumerable<Type> types)
        {
            RegisterNodeTypes(types.ToArray());
        }

        public static void RegisterNodeTypes(Assembly assembly)
        {
            RegisterNodeTypes(assembly.GetTypes());
        }
    }
}
