using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.Tests
{
    public record SelectorTestItem(int Value, string Name)
    {
        public static SelectorTestItem Zero { get; } = new(0, "Zero");
        public static SelectorTestItem One { get; } = new(1, "One");
        public static SelectorTestItem Two { get; } = new(2, "Two");
        public static SelectorTestItem Three { get; } = new(3, "Three");
        public static SelectorTestItem Four { get; } = new(4, "Four");
        public static SelectorTestItem Five { get; } = new(5, "Five");
        public static SelectorTestItem Six { get; } = new(6, "Six");
        public static SelectorTestItem Seven { get; } = new(7, "Seven");
        public static SelectorTestItem Eight { get; } = new(8, "Eight");
        public static SelectorTestItem Nine { get; } = new(9, "Nine");

        public static implicit operator SelectorTestItem(int value) => CreateArray().Single(i => i.Value == value);
        public static explicit operator int(SelectorTestItem value) => value.Value;

        public static SelectorTestItem[] CreateArray()
        {
            return new SelectorTestItem[] { Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine };
        }
    }
}
