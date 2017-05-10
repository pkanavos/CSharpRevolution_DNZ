using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Demo
{
    [TestFixture]
    class D_LocalFunctions
    {
        IEnumerable<double> Iterator_Old(int count)
        {
            if (count < 0) throw new ArgumentException(nameof(count));

            for (int i = 0; i < count; i++)
            {
                yield return Math.Pow(i, 4);
            }
        }

        IEnumerable<double> Iterator_New(int count)
        {
            if (count < 0) throw new ArgumentException(nameof(count));

            IEnumerable<double> doCount()
            {
                for (int i = 1; i <= count; i++)
                {
                    yield return Math.Pow(i,4);
                }
            }

            return doCount();
        }

        [Test]
        public void TestOld()
        {
            var iter = Iterator_Old(-5);

            foreach (var d in iter)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void TesNew()
        {
            var iter = Iterator_New(-5);

            foreach (var d in iter)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void ShouldWork()
        {
            var iter = Iterator_New(5);

            foreach (var d in iter)
            {
                Assert.That(d,Is.GreaterThan(0));
            }
        }

    }
}
