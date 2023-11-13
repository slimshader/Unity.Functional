using Bravasoft.Functional;
using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;

namespace System.Runtime.CompilerServices
{
    public class IsExternalInit { }
}
public static class FuncExt
{
    public static Func<T, TResult> Fun<T, TResult>(Func<T, TResult> f) => f;

    public static Func<T1, TResult> Then<T1, T2, TResult>(this Func<T1, T2> f1, Func<T2, TResult> f2)
        => x => f2(f1(x));

    public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(this Func<T1, T2, TResult> f)
        => x => y => f(x, y);

    public static Func<T1, Func<T2, Func<T3, TResult>>> Curry<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> f)
        => x => y => z => f(x, y, z);

    public static Func<T2, TResult> Apply<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 t1) => t2 => func(t1, t2);

    public static Func<T2, Func<T3, TResult>> Apply<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 t1)
        => t2 => t3 => func(t1, t2, t3);

    public static TResult Match<T, TResult>(this T[] ts, Func<TResult> onEmpty, Func<T, T[], TResult> onMany) =>
        ts.Length switch
        {
            0 => onEmpty(),
            _ => onMany(ts[0], ts.Skip(1).ToArray())
        };

    public static TResult Match<T, TResult>(this T[] ts, Func<TResult> onEmpty, Func<T, TResult> onOne, Func<T, T[], TResult> onMany) =>
        ts.Length switch
        {
            0 => onEmpty(),
            1 => onOne(ts[0]),
            _ => onMany(ts[0], ts.Skip(1).ToArray())
        };


}

public class FunctionalTests
{
    private int Inc(int x) => x + 1;
    private int Double(int x) => x * 2;

    [TestCase(0, 2)]
    [TestCase(1, 3)]
    [TestCase(2, 4)]
    public void ApplyTwice(int x, int expected)
    {
        var twice = FuncExt.Fun((Func<int, int> f) => f.Then(f));

        Assert.That(twice(Inc)(x).Equals(expected));
    }

    [TestCase(0, 2)]
    [TestCase(1, 4)]
    [TestCase(2, 6)]
    public void ThenCompose(int x, int expected)
    {
        var composed = FuncExt.Fun<int, int>(Inc).Then(Double);

        Assert.That(composed(x), Is.EqualTo(expected));
    }

    private static object[] FindLongestCases =
    {
        new object[] { new string[] { }, Option<string>.None},
        new object[] { new string[] { "a", "abc", "ab" }, Option<string>.Some("abc") }
    };

    [TestCaseSource(nameof(FindLongestCases))]
    public void FindLongest(string[] strings, Option<string> expected)
    {
        Option<string> FindLongest(string[] strings) =>        
            strings.Match(
                onEmpty: () => Option<string>.None,
                onMany: (string head, string[] tail) =>
                {
                    var longestInTail = FindLongest(tail);
                    return longestInTail.Where(x => x.Length >= head.Length).IfNone(head);
                }
            );
        
        Assert.That(FindLongest(strings), Is.EqualTo(expected));
    }

    private static object[] ConcatCases =
    {
        new object[] { ",", new string[] { }, ""},
        new object[] { ",", new string[] { "a" }, "a"},
        new object[] { ",", new string[] { "a", "b" }, "a,b"},
        new object[] { "--", new string[] { "a", "b", "c" }, "a--b--c"},
    };

    [TestCaseSource(nameof(ConcatCases))]
    public void ConcatWithMatch2(string separator, string[] strings, string expected)
    {
        Option<string> Concat(string[] strings) =>
            strings.Match(
                onEmpty: () => Option<string>.None,
                onMany: (string head, string[] tail) =>
                Concat(tail).Map(x => head + separator + x).IfNone(head));
        
        Assert.That(Concat(strings).IfNone(""), Is.EqualTo(expected));
    }

    [TestCaseSource(nameof(ConcatCases))]
    public void ConcatWithMatch3(string separator, string[] strings, string expected)
    {
        string Concat(string[] strings) =>
            strings.Match(
                onEmpty: () => "",
                onOne: head => head,
                onMany: (string head, string[] tail) => head + separator + Concat(tail));

        Assert.That(Concat(strings), Is.EqualTo(expected));
    }

    abstract record Tree<T>()
    {
        public static implicit operator Tree<T>(LeafType _) => new Leaf<T>();
    }

    record Leaf<T>() : Tree<T>()
    {
        public static implicit operator Leaf<T>(LeafType _) => new Leaf<T>();
    }
    record Node<T>(Tree<T> Left, T Value, Tree<T> Right) : Tree<T>();
    record LeafType();

    Tree<T> node<T>(Tree<T> left, T value, Tree<T> right) => new Node<T>(left, value, right);
    LeafType leaf() => new LeafType();

    [Test]
    public void TreeHeight()
    {
        var tree = node(node(leaf(), 2, leaf()), 1, node(node(leaf(), 4, leaf()), 3, leaf()));

        int Height(Tree<int> tree) => tree switch
        {
            Leaf<int> => 0,
            Node<int>(var l, _, var r) => 1 + Math.Max(Height(l), Height(r)),
            _ => throw new Exception()
        };

        Assert.That(Height(tree), Is.EqualTo(3));
    }

    abstract record Nat;
    record Zero : Nat;
    record Succ(Nat n) : Nat;

    [Test]
    public void PredecessorNat()
    {
        Option<Nat> Pred(Nat n) => n switch
        {
            Zero => Option<Nat>.None,
            Succ(var n1) => n1,
            _ => throw new Exception()
        };

        Assert.That(Pred(new Zero()), Is.EqualTo(Option<Nat>.None));
        Assert.That(Pred(new Succ(new Zero())), Is.EqualTo(Option<Nat>.Some(new Zero())));
        Assert.That(Pred(new Succ(new Succ(new Zero()))), Is.EqualTo(Option<Nat>.Some(new Succ(new Zero()))));
    }

    [Test]
    public void AddNat()
    {
        Nat Add(Nat a, Nat b) => b switch
        {
            Succ(var b1) => Add(new Succ(a), b1),
            Zero => a,
            _ => throw new Exception()
        };

        Assert.That(Add(new Zero(), new Zero()), Is.EqualTo(new Zero()));
        Assert.That(Add(new Zero(), new Succ(new Zero())), Is.EqualTo(new Succ(new Zero())));
        Assert.That(Add(new Succ(new Zero()), new Succ(new Zero())), Is.EqualTo(new Succ(new Succ(new Zero()))));
    }

    readonly struct Either<TLeft, TRight>
    {
        readonly TLeft _left;
        readonly TRight _right;
        readonly bool _isRight;

        public Either(TLeft left)
        {
            _left = left;
            _right = default;
            _isRight = false;
        }

        public Either(TRight right)
        {
            _left = default;
            _right = right;
            _isRight = true;
        }

        public Either<TLeft, URight> Map<URight>(Func<TRight, URight> map) =>
            _isRight ? new Either<TLeft, URight>(map(_right)) : new Either<TLeft, URight>(_left);
    }

    private static U FoldTree<T, U>(Func<U, T, U, U> f, U init, Tree<T> tree) =>
        tree switch
        {
            Leaf<T> => init,
            Node<T>(var l, var v, var r) => f(FoldTree(f, init, l), v, FoldTree(f, init, r)),
            _ => throw new Exception()
        };

    [Test]
    public void TreeToList()
    {
        var tree = node(node(leaf(), 2, leaf()), 1, node(node(leaf(), 4, leaf()), 3, leaf()));

        // MINE
        //T[] TreeToList<T>(Tree<T> tree) => tree switch
        //{
        //    Leaf<T> => Array.Empty<T>(),
        //    Node<T>(var l, var v, var r) => new T[] { v }.Concat(TreeToList(l)).Concat(TreeToList(r)).ToArray(),
        //    _ => throw new Exception()
        //};


        T[] TreeToList<T>(Tree<T> tree) =>
            FoldTree<T, T[]>((l, x, r) => new[] {x}.Concat(l).Concat(r).ToArray(), new T [0], tree);

        Assert.That(TreeToList(tree), Is.EqualTo(new int[] { 1, 2, 3, 4 }));
    }

}
