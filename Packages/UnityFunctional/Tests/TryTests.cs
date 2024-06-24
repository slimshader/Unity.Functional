using FluentAssertions;
using NUnit.Framework;
using System;
using UnityEngine;

namespace Bravasoft.Functional.Tests
{
    [TestFixture]
    public class TryTests : MonoBehaviour
    {
        [Test]
        public void CantInitializeWithNullFunc()
        {
            Action action = () => new Try<int>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void IfExceptionRetyrnsValueIfValid()
        {
            var @try = new Try<int>(() => 1);

            var value = @try.IfException(-1);

            value.Should().Be(1);
        }

        [Test]
        public void CastingValidTryToResultYieldsnOkResult()
        {
            var @try = Try.Of(() => 1);

            var result = @try.ToResult();

            result.IsOk.Should().BeTrue();
        }

        [Test]
        public void CastingInvalidTryToResultYieldsFailedResult()
        {
            var @try = Try.Of<int>(() => throw new Exception());

            var result = @try.ToResult();

            result.IsOk.Should().BeFalse();

            result.IsError<ExceptionError>().Should().BeTrue();
        }

        [Test]
        public void MappingValidTryYiledsValidMappedValue()
        {
            var @try = Try.Of(() => 1);

            var mapped = @try.Map(x => x + 1);

            var value = mapped.IfException(-1);

            value.Should().Be(2);
        }

        [Test]
        public void CanMatchValueState()
        {
            var @try = Try.Of(() => 1);

            var result = @try.Match(
                onValue: x => x + 1,
                onException: e => -1
            );

            result.Should().Be(2);
        }

        [Test]
        public void CanMatchExceptionState()
        {
            var @try = Try.Of<int>(() => throw new Exception());

            var result = @try.Match(
                onValue: x => x + 1,
                onException: e => -1
            );

            result.Should().Be(-1);
        }

        [Test]
        public void CanMapWithLinqSelect()
        {
            var mapped = from x in Try.Of(() => 1)
                         select x + 1;

            var value = mapped.IfException(-1);

            value.Should().Be(2);
        }

        [Test]
        public void CanUseOnExceptionFallback()
        {
            var @try = Try.Of<int>(() => throw new Exception());

            var result = @try.IfException(-1);

            result.Should().Be(-1);
        }

        [Test]
        public void BindingTwoValidTriesYieldsValidResult()
        {
            var result = Try.Of(() => 2)
                .Bind(x => Try.Of(() => x + 1));

            var value = result.IfException(-1);

            value.Should().Be(3);
        }

        [Test]
        public void BindingValidAndExcpetionYieldInvalidResult()
        {
            var result = Try.Of(() => 2)
                .Bind(x => Try.Of<int>(() => throw new Exception()));

            var value = result.IfException(-1);

            value.Should().Be(-1);
        }

        [Test]
        public void BindingExceptionAndValidYieldsInvalidResult()
        {
            var result = Try.Of<int>(() => throw new Exception())
                .Bind(x => Try.Of(() => x + 1));

            var value = result.IfException(-1);

            value.Should().Be(-1);
        }

        [Test]
        public void CanBindWithLinq()
        {
            var result = from x in Try.Of(() => 2)
                         from y in Try.Of(() => x + 1)
                         select y;

            var value = result.IfException(-1);

            value.Should().Be(3);
        }

        [Test]
        public void CanWrapIntParse()
        {
            var result = from x in Try.Of(() => int.Parse("1"))
                         from y in Try.Of(() => int.Parse("2"))
                         select x + y;

            var value = result.IfException(-1);

            value.Should().Be(3);
        }
    }
}
