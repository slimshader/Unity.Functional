using Bravasoft.Functional;
using NUnit.Framework;
using System.Threading.Tasks;
using UnityEngine;
using static Bravasoft.Functional.Prelude;

public class AsyncOptionTests : MonoBehaviour
{
    [Test]
    public async Task IsSomeIsFalseByDefault()
    {
        AsyncOption<int> option = default;

        var isSome = await option.IsSome;

        Assert.That(isSome, Is.False);
    }

    [Test]
    public async Task IsSomeIsTrueWhenInitializedWithValue()
    {
        AsyncOption<int> option = AsyncOption<int>.Some(1);

        var isSome = await option.IsSome;

        Assert.That(isSome, Is.True);
    }

    [Test]
    public async Task MappedSomeIsSome()
    {
        var option = AsyncSome(1);

        var asyncMapped = option.Map(x => x + 1);

        var mapped = await asyncMapped;

        Assert.That(mapped.IsSome, Is.True);
    }

    [Test]
    public async Task MappedNoneIsNone()
    {
        AsyncOption<int> option = default;

        var asyncMapped = option.Map(x => x + 1);

        var mapped = await asyncMapped;

        Assert.That(mapped.IsSome, Is.False);
    }

    [Test]
    public async Task BoundSomeIsSome()
    {
        var option = AsyncSome(1);

        var asyncBound = option.Bind(x => AsyncSome(x + 1));

        var bound = await asyncBound;

        Assert.That(bound.IsSome, Is.True);
        Assert.That(bound.IfNoneDefault(), Is.EqualTo(2));
    }

    [Test]
    public async Task BoundNoneIsNone()
    {
        AsyncOption<int> option = default;

        var asyncBound = option.Bind(x => AsyncSome(x + 1));

        var bound = await asyncBound;

        Assert.That(bound.IsSome, Is.False);
    }

    [Test]
    public async Task CanMapSomeWithLinq()
    {
        var option = AsyncSome(1);

        var mapped = await (from x in option select x + 1);

        Assert.That(mapped.IsSome, Is.True);
        Assert.That(mapped.IfNoneDefault(), Is.EqualTo(2));
    }
}
