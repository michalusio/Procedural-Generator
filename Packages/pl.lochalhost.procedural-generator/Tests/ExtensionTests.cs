using NUnit.Framework;
using Packages.pl.lochalhost.procedural_generator.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Math.ArithmeticNode;
using static Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Math.RoundNode;
using static Packages.pl.lochalhost.procedural_generator.Editor.Nodes.Meshes.SubdivisionNode;

namespace lochalhost.procedural_generator.Editor.Tests
{
    public class ExtensionTests
    {
        public static Color[] Colors = new[] {
            Color.red
        };

        [TestCaseSource(nameof(Colors))]
        public void BorderColorShouldChangeEveryBordersColor(Color borderColor)
        {
            // Arrange
            var element = new VisualElement();
            
            // Act
            element.style.BorderColor(borderColor);

            // Assert
            Helper.AreEqual(borderColor, element.style.borderBottomColor.value);
            Helper.AreEqual(borderColor, element.style.borderTopColor.value);
            Helper.AreEqual(borderColor, element.style.borderLeftColor.value);
            Helper.AreEqual(borderColor, element.style.borderRightColor.value);
        }

        [TestCase(3.5f)]
        public void BorderWidthShouldChangeEveryBordersWidth(float borderWidth)
        {
            // Arrange
            var element = new VisualElement();

            // Act
            element.style.BorderWidth(borderWidth);

            // Assert
            Assert.AreEqual(borderWidth, element.style.borderBottomWidth.value);
            Assert.AreEqual(borderWidth, element.style.borderTopWidth.value);
            Assert.AreEqual(borderWidth, element.style.borderLeftWidth.value);
            Assert.AreEqual(borderWidth, element.style.borderRightWidth.value);
        }

        [TestCase(3.5f)]
        public void BorderRadiusShouldChangeEveryBordersRadius(float borderRadius)
        {
            // Arrange
            var element = new VisualElement();

            // Act
            element.style.BorderRadius(borderRadius);

            // Assert
            Assert.AreEqual(borderRadius, element.style.borderTopLeftRadius.value.value);
            Assert.AreEqual(borderRadius, element.style.borderTopRightRadius.value.value);
            Assert.AreEqual(borderRadius, element.style.borderBottomLeftRadius.value.value);
            Assert.AreEqual(borderRadius, element.style.borderBottomRightRadius.value.value);
        }

        [TestCase(Operation.Addition)]
        [TestCase(RoundingMode.Round)]
        [TestCase(SubdivisionType.Simple)]
        public void ShouldGetEnumValues<T>(T item) where T : Enum
        {
            Assert.That(Extensions.Values<T>(), Is.All.AssignableTo(typeof(T)));
        }

        [TestCase(typeof(int), ExpectedResult = "int32")]
        [TestCase(typeof(float), ExpectedResult = "single")]
        [TestCase(typeof(Extensions), ExpectedResult = "extensions")]
        [TestCase(typeof(IEnumerable<IEnumerable<int>>), ExpectedResult = "ienumerable_1__ienumerable_1__int32")]
        public string ShouldGetClassName(Type type)
        {
            return type.GetClassName();
        }

        [TestCase(typeof(int), ExpectedResult = "int")]
        [TestCase(typeof(float), ExpectedResult = "float")]
        [TestCase(typeof(Extensions), ExpectedResult = "Extensions")]
        [TestCase(typeof(IDictionary<Dictionary<double, bool>, string>), ExpectedResult = "IDictionary<Dictionary<double, bool>, string>")]
        public string ShouldPrettifyName(Type type)
        {
            return type.PrettyName();
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(6)]
        [TestCase(12)]
        public void ShouldGroupItems(int groupCount)
        {
            // Arrange
            var items = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Act
            var grouped = items.GroupInto(groupCount).ToList();

            // Assert
            Assert.That(grouped, Is.All.Length.EqualTo(groupCount));
            Assert.That(grouped, Is.All.SubsetOf(items));
            Assert.That(grouped, Has.Count.EqualTo(items.Length / groupCount));
        }

        [TestCase(5)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(11)]
        public void ShouldThrowWhenGrouping(int groupCount)
        {
            // Arrange
            var items = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Act && Assert
            Assert.Throws(typeof(ArgumentException), () => items.GroupInto(groupCount).ToList());
        }
    }
}
