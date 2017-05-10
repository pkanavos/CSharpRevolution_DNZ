using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Demo
{
    [TestFixture]
    class ValueTuples
    {
        (string first, string last) LookupName(long id) => (first: "a", last: $"b {id}");

        [Test]
        public void CallAsVar()
        {
            var result = LookupName(4);

            Assert.That(result.last,Is.EqualTo("b 4"));
        }

        [Test]
        public void CallDeconstructed()
        {
            var (first,last)= LookupName(4);

            Assert.That(last, Is.EqualTo("b 4"));
        }

        [Test]
        public void CallAsStrings()
        {
            (string first, string last) = LookupName(4);

            Assert.That(last, Is.EqualTo("b 4"));
        }

        [Test]
        public void CallAs_Dr_Evil()
        {
            (_, string last) = LookupName(4);

            Assert.That(last, Is.EqualTo("b 4"));
        }

        [Test]
        public void ValueEquality()
        {
            var (x, y) = (100, 200);
            Assert.That((x, y), Is.EqualTo((100, 200)));

        }
    }
}
