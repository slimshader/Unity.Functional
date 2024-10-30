using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Bravasoft.Functional.Tests
{
    public static class OptionTestingExtensions
    {
        public sealed class OptionAssertions<T> : ObjectAssertions<Option<T>, OptionAssertions<T>>
        {
            public OptionAssertions(Option<T> subject) : base(subject)
            {
            }

            protected override string Identifier => "Option<T>";

            public AndConstraint<OptionAssertions<T>> BeSome(T expectedValue)
            {
                Execute.Assertion
                    .ForCondition(Subject.IsSome)
                    .FailWith("Expected {context:option} to be Some, but it was None.");
                Execute.Assertion
                    .ForCondition(Subject.TryGetValue(out var value) && value.Equals(expectedValue))
                    .FailWith("Expected {context:option} to be Some with value {0}, but it was Some with value {1}.",
                        expectedValue, value);
                return new AndConstraint<OptionAssertions<T>>(this);
            }

            public AndConstraint<OptionAssertions<T>> BeNone()
            {
                Execute.Assertion
                    .ForCondition(Subject.IsNone)
                    .FailWith("Expected {context:option} to be None, but it was Some.");
                return new AndConstraint<OptionAssertions<T>>(this);
            }
        }

        public static OptionAssertions<T> Should<T>(this Option<T> actualValue)
        {
            return new OptionAssertions<T>(actualValue);
        }
    }
}
