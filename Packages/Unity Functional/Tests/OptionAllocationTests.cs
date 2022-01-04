using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools.Constraints;
using Is = UnityEngine.TestTools.Constraints.Is;

namespace Bravasoft.Tests
{
    public class OptionAllocationTests
    {
        class Dummy { }

        [Test]
        public void ListFirstOrNoneForReferenceTypes()
        {
            Option<Dummy> o = null;
            Func<Dummy, bool> predicate = _ => true;
            var list = new List<Dummy>();

            TestDelegate block = () =>
            {
                o = list.FirstOrNone(predicate);
            };

            Assert.That(block, Is.Not.AllocatingGCMemory());

            Assert.That(o.IsSome, Is.False);
        }
    }
}
