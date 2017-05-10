using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Demo
{
    static class DeconstructorExtensions
    {
        public static void Deconstruct<K, V>(this KeyValuePair<K, V> pair, out K key, out V value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }

    class MyPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public MyPoint(int x, int y)
        {
            X=x;
            Y=y;
        }
        public void Deconstruct(out int x,out int y)
        {
            x = X;
            y = Y;
        }
    }

    [TestFixture]
    class Deconstructors
    {
        [Test]
        public void DeconstructPoint()
        {
            var (x, y) = new MyPoint(100, 200);

            Assert.That(x,Is.EqualTo(100));
        }


        [Test]
        public void Iterate_Dictionary()
        {
            var dict = new Dictionary<string, int>
            {
                ["a"] = 4,
                ["b"] = 10,
            };

            foreach (var(key, value) in dict)
            {
                Console.WriteLine(key);
            }

            Assert.Pass();
        }
    }
}
