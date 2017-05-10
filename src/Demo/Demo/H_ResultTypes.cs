using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Demo
{
    public interface IResult<TSuccess, TFailure> { }
    public class Success<TSuccess, TFailure> : IResult<TSuccess, TFailure>
    {
        public TSuccess Result { get; }

        //Expression bodied constructor!
        public Success(TSuccess it) => Result= it;

        public void Deconstruct(out TSuccess result)=>result = Result;

    }
    public class Failure<TSuccess, TFailure> : IResult<TSuccess, TFailure>
    {
        public TFailure Error { get; }

        //Expression bodied constructor!
        public Failure(TFailure it) => Error = it;

        public void Deconstruct(out TFailure error) => error= Error;
    }


    [TestFixture()]
    class H_ResultTypes
    {
        public IResult<int, string> DoSomething(bool ok)
        {
            if (ok) return new Success<int, string>(199);
            return new Failure<int, string>("Ouch");
        }


        [TestCase(true)]
        [TestCase(false)]
        public void WorksWithResult(bool ok)
        {

            var result = DoSomething(ok);
            switch (result)
            {
                case Success<int, string> c:
                        Assert.That(c.Result, Is.EqualTo(199));
                        break;
                case Failure<int, string> c:
                    //DOESNT WORK: Assert.That(c.Result, Is.EqualTo(199));
                    Assert.That(c.Error, Is.EqualTo("Ouch"));
                    break;
                default: throw new InvalidOperationException("Missed match");
            }
        }
    }
}
