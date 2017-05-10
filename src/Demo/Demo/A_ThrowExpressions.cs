using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Demo
{
    class MyClass
    {
        public string Value => "Tada";
    }

    [TestFixture]
    class ThrowExpressions
    {
        public string MyMethod(MyClass someParam)
        {
            var myVar = someParam ?? throw new ArgumentException(nameof(someParam));
            return someParam.Value;
        }

        [Test]
        public void ThrowsOnNull()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                MyMethod(null);
            });
        }

        [Test]
        public void WorksOnValidClass()
        {
            var result=MyMethod(new MyClass());

            Assert.That(result, Is.EqualTo("Tada"));
        }

    }
}
