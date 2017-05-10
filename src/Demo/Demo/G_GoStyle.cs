using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Demo
{
    [TestFixture]
    class G_GoStyle
    {
        (string result, string error) TheQuestion(bool vogonsArrived)
        {
            if (vogonsArrived)
            {
                return (null, "Goodbye and thanks for all the fish");
            }
            else
            {
                return ("How much is 6 by 8?", null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ItsCool(bool vogons)
        {
            var (result, error) = TheQuestion(vogons);
            if (error != null)
            {
                Assert.Fail(error);
            }
            Assert.That(result, Is.EqualTo("How much is 6 by 8?"));
        }


    }
}
