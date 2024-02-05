using FluentAssertions;
using NUnit.Framework;
using Bravasoft.Functional;

using static Bravasoft.Functional.Prelude;
using System;

namespace Bravasoft.Functional
{
    public sealed class ResultMatcher<T>
    {
        Action<T> _onOk;
        Action<Error> _onError;

        public ResultMatcher<T> OnOk(Action<T> onOk)
        {
            _onOk = onOk;
            return this;
        }

        public ResultMatcher<T> OnError(Action<Error> onError)
        {
            _onError = onError;
            return this;
        }

        public Unit Match(in Result<T> result)
        {
            result.BiIter(_onOk, _onError);
            return default;
        }
    }

    public static class ResultExtensions
    {
        public class AutoResultMatcher<T>
        {
            private readonly Result<T> _result;
            private readonly ResultMatcher<T> _matcher;

            public AutoResultMatcher(Result<T> result, ResultMatcher<T> matcher)
            {
                _result = result;
                _matcher = matcher;
            }

            public Unit Match() => _matcher.Match(_result);
        }

        public static AutoResultMatcher<T> Matcher<T>(this in Result<T> result, Action<ResultMatcher<T>> onMatcherSetup)
        {
            var matcher = new ResultMatcher<T>();
            onMatcherSetup(matcher);
            return new AutoResultMatcher<T>(result, matcher);
        }
    }
}

public class ResultMatcherTests
{
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void CanMatchOk(int value)
    {
        var ok = Ok(value);

        var matchedValue = 0;

        ok.Matcher(m =>
        {
            m.OnOk(v => matchedValue = v);
        }).Match();

        matchedValue.Should().Be(value);
    }

    [Test]
    public void CanReuseOkMatcher()
    {        
        var Oks = 0;

        var matcher = new ResultMatcher<int>()
            .OnOk(v => Oks++);

        var ok1 = Ok(1);
        var ok2 = Ok(1);

        matcher.Match(ok1);
        matcher.Match(ok2);

        Oks.Should().Be(2);
    }

    [Test]
    public void CanMatchFail()
    {
        var fail = Fail<int>("error");

        var matchedMessage = string.Empty;

        fail.Matcher(m =>
        {
            m.OnError(e => matchedMessage = e.Message);
        }).Match();

        matchedMessage.Should().Be("error");
    }
}
