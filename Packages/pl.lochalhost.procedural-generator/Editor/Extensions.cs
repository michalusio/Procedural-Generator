using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor
{
    internal static class Extensions
    {
        public static void BorderColor(this IStyle style, StyleColor color)
        {
            style.borderBottomColor = color;
            style.borderTopColor = color;
            style.borderRightColor = color;
            style.borderLeftColor = color;
        }

        public static void BorderWidth(this IStyle style, StyleFloat width)
        {
            style.borderBottomWidth = width;
            style.borderTopWidth = width;
            style.borderRightWidth = width;
            style.borderLeftWidth = width;
        }

        public static void BorderRadius(this IStyle style, StyleLength radius)
        {
            style.borderTopLeftRadius = radius;
            style.borderTopRightRadius = radius;
            style.borderBottomLeftRadius = radius;
            style.borderBottomRightRadius = radius;
        }

        public static IEnumerable<T[]> GroupInto<T>(this IEnumerable<T> enumerable, int groupCount)
        {
            if (groupCount < 1) throw new ArgumentException("Enumerable count not divisible by group count");
            var group = new List<T>(groupCount);
            foreach (var item in enumerable)
            {
                group.Add(item);
                if (group.Count == groupCount)
                {
                    yield return group.ToArray();
                    group.Clear();
                }
            }
            if (group.Any()) throw new ArgumentException("Enumerable count not divisible by group count");
        }

        public static List<T> Values<T>() where T: Enum
        {
            return Enum.GetValues(typeof(T)).OfType<T>().ToList();
        }

        public static string GetClassName(this Type t)
        {
            return t.Name.ToLower().Replace('`', '_') + string.Join("", t.GetGenericArguments().Select(g => "__" + g.GetClassName()));
        }

        public static string PrettyName(this Type type)
        {
            if (type.GetGenericArguments().Length == 0)
            {
                return NatifyName(type.Name);
            }
            var genericArguments = type.GetGenericArguments();
            var typeDefinition = type.Name;
            var unmangledName = typeDefinition.Substring(0, typeDefinition.IndexOf("`"));
            
            return NatifyName(unmangledName) + "<" + string.Join(",", genericArguments.Select(PrettyName)) + ">";
        }

        private static string NatifyName(string unmangledName)
        {
            switch (unmangledName)
            {
                case "Int32":
                    return "int";
                case "Int64":
                    return "long";
                case "Double":
                    return "double";
                case "Single":
                    return "float";
                case "String":
                    return "string";
                case "Boolean":
                    return "bool";
                case "Byte":
                    return "byte";
                case "Object":
                    return "object";
                default:
                    return unmangledName;
            }
        }
    }
}
