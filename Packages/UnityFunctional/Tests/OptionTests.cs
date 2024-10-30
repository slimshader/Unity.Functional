using Bravasoft.Functional.Json;
using Bravasoft.Functional.Unity;
using FluentAssertions;
using FluentAssertions.Numeric;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;
using static Bravasoft.Functional.Prelude;

namespace Bravasoft.Functional.Tests
{
    public class OptionTests
    {
        [Test]
        public void IsNoneByDefaut()
        {
            var option = new Option<int>();

            option.IsSome.Should().BeFalse();
        }

        [Test]
        public void NullReferenceOptionalIsNone()
        {
            var option = Prelude.Optional((object)null);

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
            var oi = from x in Some(1)
                     select x + 1;

            Assert.That((int)oi, Is.EqualTo(2));
        }

        [Test]
        public void LinqSelectManySupport()
        {
            var oi = from x in Some(1)
                     from y in Some(2)
                     select x + y;

            Assert.That((int)oi, Is.EqualTo(3));
        }

        [Test]
        public void LinqWhereSupport()
        {
            var io1 = from x in Some(1)
                      where x > 2
                      select x;

            var io2 = from x in Some(3)
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
            var o1 = Some(1);
            var o2 = Some(2);
            var o11 = Some(1);
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

        [Test]
        public void ComparisonToSameSomeValueYielcsTrue()
        {
            var opt5 = Some(5);

            Assert.IsTrue(opt5 == 5);
        }

        [Test]
        public void ComparisonToDifferentSomeValueYielcsTrue()
        {
            var opt5 = Some(5);

            Assert.IsFalse(opt5 == 4);
        }

        class MyClass
        {
            public string Prop { get; set; }
        }

        [Test]
        public void CanMapOptionallyNullProperty()
        {
            var c = Some(new MyClass());

            var p = c.MapOptional(x => x.Prop);

            Assert.That(!p.IsSome);
        }

        class Base { }
        class Derived : Base { }

        [Test]
        public void CanCast()
        {
            var d = Some<Base>(new Derived());

            Assert.That(d.TryCast<Derived>().IsSome);
        }

        [Test]
        public void CanForeach()
        {
            var d = Some(1);
            var count = 0;

            foreach (var i in d)
            {
                count++;
            }

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void CanForeachNone()
        {
            var d = Option<int>.None;
            var count = 0;

            foreach (var i in d)
            {
                count++;
            }

            count.Should().Be(0);
        }

        [Test]
        public void CanFoldSome()
        {
            var d = Some(1);
            var result = d.Fold(1, (acc, x) => acc + x);

            result.Should().Be(2);
        }

        [Test]
        public void CanFoldNone()
        {
            var d = Option<int>.None;
            var result = d.Fold(1, (acc, x) => acc + x);

            result.Should().Be(1);
        }

        [Test]
        public void CanUseShortCircuitLogicalOrOperator()
        {
            var d1 = Some(1);
            var d2 = Option<int>.None;

            var result = d1 || d2;

            result.Should().Be(d1);
        }

        [Test]
        public void CanUseShortCircuitLogicalOrOperatorWithNone()
        {
            var d1 = Option<int>.None;
            var d2 = Option<int>.None;

            var result = d1 || d2;

            result.Should().Be(d2);
        }

        [Test]
        public void CanUseShortCircuitLogicalAndOperator()
        {
            var d1 = Option.None<int>();
            var d2 = Some(2);

            var result = d1 || d2;

            result.Should().Be(2);
        }

        class TestClass
        {
            public Option<int> Value { get; set; }
        }




        [Test]
        public void CanDeserializeValueFromJson()
        {
            var json = "{\"Value\": 1}";

            var options = new JsonSerializerOptions()
                .AddOptionConverter();

            var testObject = JsonSerializer.Deserialize<TestClass>(json, options);

            testObject.Value.Should().BeSome(1);
        }

        [Test]
        public void CanDeserializeNullFromJson()
        {
            var json = "{\"Value\": null}";

            var options = new JsonSerializerOptions()
                .AddOptionConverter();

            var testObject = JsonSerializer.Deserialize<TestClass>(json, options);
            testObject.Value.Should().BeNone();
        }
    }
}
