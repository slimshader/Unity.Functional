using FluentAssertions;
using NUnit.Framework;

namespace Bravasoft.Functional.Tests
{
    public readonly struct Any
    {
        private readonly bool _value;

        public Any(bool value)
        {
            _value = value;
        }

        public bool Value => _value;

        public Any Append(Any other) => new Any(_value || other._value);
    }

    public static class AnyExtensions
    {
        public static Any AsAny(this bool value) => new Any(value);
    }

    public class AnyTests
    {
        [Test]
        // can append Any to other Any
        public void AppendingTrueAndFalseResultsInTrue()
        {
            var any1 = true.AsAny();
            var any2 = false.AsAny();

            var result = any1.Append(any2);

            result.Value.Should().BeTrue();
        }
    }
}
