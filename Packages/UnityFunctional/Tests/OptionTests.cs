using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Bravasoft.Functional.Unity;

namespace Bravasoft.Functional.Tests
{
    public class OptionTests
    {
        [Test]
        public void IsNoneByDefaut()
        {
            var option = new Option<int>();

            Assert.That(option.IsSome, Is.False);
        }

        [Test]
        public void NullReferenceOptionalIsNone()
        {
            var option = Prelude.Optional((object)null);

            Assert.That(option.IsSome, Is.False);
        }

        [Test]
        public void IsNoneWhenGettingMissingComponent()
        {
            var go = new GameObjectBuilder().Build();

            var option = go.TryGetComponent<Collider>();

            Assert.That(option.IsSome, Is.False);
        }


        [Test]
        public void IsSomeWhenGettingComponent()
        {
            var go = new GameObjectBuilder()
                .WithComponent<BoxCollider>()
                .Build();

            var option = go.TryGetComponent<Collider>();

            Assert.That(option.IsSome, Is.True);
            Assert.That(option.Match(x => x.GetType(), null), Is.EqualTo(typeof(BoxCollider)));
        }

        [Test]
        public void IsNoneWhenGettingMissingComponentInParent()
        {
            var parent = new GameObject();
            var go = new GameObject();
            go.transform.SetParent(parent.transform);

            var option = go.TryGetComponentInParent<Collider>();

            Assert.That(option.IsSome, Is.False);
        }

        [Test]
        public void IsSomeWhenGettingComponentInParent()
        {
            var parent = new GameObject();
            parent.AddComponent<BoxCollider>();

            var go = new GameObject();
            go.transform.SetParent(parent.transform);

            var option = go.TryGetComponentInParent<Collider>();

            Assert.That(option.IsSome, Is.True);
            Assert.That(option.Match(x => x.GetType(), null), Is.EqualTo(typeof(BoxCollider)));
        }

        [Test]
        public void IsNoneWhenGettingMissingComponentInChildren()
        {
            var go = new GameObject();
            var child = new GameObject();
            child.transform.SetParent(go.transform);

            var option = go.TryGetComponentInChildren<Collider>();

            Assert.That(option.IsSome, Is.False);
        }

        [Test]
        public void IsSomeWhenGettingComponentInChildren()
        {
            var go = new GameObject();
            var child = new GameObject();
            child.transform.SetParent(go.transform);
            child.AddComponent<BoxCollider>();

            var option = go.TryGetComponentInChildren<Collider>();

            Assert.That(option.IsSome, Is.True);
        }

        [Test]
        public void LinqSelectSupport()
        {
            var oi = from x in Option.Some(1)
                     select x + 1;

            Assert.That((int)oi, Is.EqualTo(2));
        }

        [Test]
        public void LinqSelectManySupport()
        {
            var oi = from x in 1.ToSome()
                     from y in 2.ToSome()
                     select x + y;

            Assert.That((int)oi, Is.EqualTo(3));
        }

        [Test]
        public void LinqWhereSupport()
        {
            var io1 = from x in 1.ToSome()
                      where x > 2
                      select x;

            var io2 = from x in 3.ToSome()
                      where x > 2
                      select x;

            Assert.That(io1.IsSome, Is.False);
            Assert.That((int)io2, Is.EqualTo(3));
        }

        [Test]
        public void DictionarySupport()
        {
            var dict = new Dictionary<int, int>();

            var o1 = dict.TryGetValue(1);

            dict.Add(1, 3);

            var o2 = dict.TryGetValue(1);

            Assert.That(o1.IsSome, Is.False);
            Assert.That((int)o2, Is.EqualTo(3));
        }

        [Test]
        public void Equality()
        {
            var o1 = Option.Some(1);
            var o2 = Option.Some(2);
            var o11 = Option.Some(1);
            var n = Option<int>.None;

            Assert.That(o1, Is.EqualTo(o11));
            Assert.That(o2, Is.Not.EqualTo(o1));
            Assert.That(o1, Is.Not.EqualTo(n));
        }

        class Dummy { }

        [Test]
        public void DefaultConversion()
        {
            var dummy = new Dummy();

            Option<Dummy> o1 = dummy;

            Assert.That((Dummy)o1, Is.EqualTo(dummy));
        }
    }
}
