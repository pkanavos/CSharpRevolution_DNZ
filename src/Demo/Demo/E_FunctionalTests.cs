using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Demo
{
    [TestFixture]
    class E_FunctionalTests
    {
        [Test]
        public void CanLoadFromExistingFile()
        {
            var expected = new DateTime(2017, 4, 1);

            var result = Cutoff.LoadFrom("existingFile", exists => true, read => "2017-04-01");

            Assert.That(result, Is.EqualTo(expected));

        }

        [Test]
        public void MissingFileReturnsMinDate()
        {
            var result = Cutoff.LoadFrom("noFile", exists => false);

            Assert.That(result, Is.EqualTo(DateTime.MinValue));

        }

    }
}
