using FluentAssertions;
using NUnit.Framework;
using Bravasoft.Functional;

using static Bravasoft.Functional.Prelude;

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

    class CustomError<T> : Error
    {
        public CustomError(T data)
        {
            Data = data;
        }

        public T Data { get; }
    }

    [Test]
    public void CanMatchSpecificErrorType()
    {
        var failed = Result<int>.Fail(new CustomError<int>(1));

        var matchedData = 0;

        failed.Matcher(m =>
        {
            m.OnError<CustomError<int>>(e => matchedData = e.Data);
        }).Match();

        matchedData.Should().Be(1);
    }

    [Test]
    public void WillMatchSpecificErrorBeforeGeneral()
    {
        var failed = Result<int>.Fail(new CustomError<int>(1));

        var matchedData = 0;

        failed.Matcher(m =>
        {
            m.OnError<CustomError<int>>(e => matchedData = e.Data);
            m.OnError(e => matchedData = 2);

        }).Match();

        matchedData.Should().Be(1);
    }

    [Test]
    public void WillUseeFallbackIfNotMatched()
    {
        Result<int> r = default;

        var called = false;
        r.Matcher(m => { m.OnFallback(() => called = true); })
            .Match();

        called.Should().BeTrue();
    }
}