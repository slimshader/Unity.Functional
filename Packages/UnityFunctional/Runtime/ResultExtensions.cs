using System;

namespace Bravasoft.Functional
{
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
