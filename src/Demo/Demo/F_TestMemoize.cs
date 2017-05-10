using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Demo
{
    [TestFixture]
    class F_TestMemoize
    {
        public Func<TIn, TOut> Memoize<TIn, TOut>(Func<TIn, TOut> f)
        {
            var cache = new Dictionary<TIn, TOut>();
            TOut Run (TIn x)

            {
                if (cache.ContainsKey(x))
                {
                    return cache[x];
                }
                Console.WriteLine("Working");
                var result = f(x);
                cache[x] = result;
                return result;

                //OR
                //return cache.TryGetValue(x, out TOut result)
                //    ? result
                //    : (cache[x] = f(x));

            }

            return Run;

        }

        [Test]
        public void TestMemo()
        {
            string MyFunc(int i)
            {
                return $"Result is i";
            }

            var f = Memoize<int,string>(MyFunc);

            var result1=f(5);
            var result2 = f(5);


        }

    }
}
