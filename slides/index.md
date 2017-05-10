- title : C# 7 - Prelude to the Revolution
- description :# 7 - Prelude to the Revolution
- author : Panagiotis Kanavos
- theme : night
- transition : default

# C# 7 

![FsReveal](images/CSharp_Revolution.jpg)

##Prelude to the 
##Functional Revolution

*** 

### Lots of "small" changes 
with some big consequences

***

### A short list

- Throw Expressions
- Value Tuples and Multiple return values
- Decomposition
- Out variables
- Pattern Matching
- Local Functions

*** 

### Throw Expressins
On the road to Expressions

- Expressions return values. Statements don't
- Functional languages use expressions *a lot*
- throw is now an expression
- Assignment is also an expression

*** 

### This is now possible

    [lang=CS]
    public void MyMethod(MyClass someParam)
    {
        var myVar = someParam ?? throw new ArgumentException(nameof(name));
    }

*** 

### Value Tuples - Lazy or Crazy?

- Tuples with names
- Value types - no allocation penalties
- Work where anonymous types don't
- Compiler magic

***

	[lang=CS]
	    (string first, string last) LookupName(long id){ return (first:"a", last:"b");}
        //or
	    (string first, string last) LookupName(long id) => (first:"a", last:"b");

Use

    [lang=CS]	
	    var result=LookupName(4);
	    Console.WriteLine(result.first);
	    //or
	    var (first,last) =LookupName(4);
	    Console.WriteLine(first);


*** 

### Why not build a class?

Because :

- Very easy to define
- *Value Equality* - no need for Equals, GetHashCode etc.
- No allocations, no GC

***

### Caveats

- Compiler magic -> No real names
- No reflection
- Can't use in public APIs
- Structs -> Lots of copying

Solved with records in C# 8+

    [lang=CS]
    class Point(int X, int Y, int Z);


***

### Deconstruction

![Stinking](images\Deconstruction.jpg)

*** 

### What?

- Deconstruct tuples
- Or any type with a `void Deconstruct(out)` method
- Works with extension methods too
- `Variable not used` warning if a variable isn't used



*** 

### Where are my keys?

    [lang=cs]
    public static void Decompose<T,V>(this IDictionary<T,V> pair,out T key, out V value)
    {key=pair.Key; value=pair.Value; }

    foreach(var (key,pair) in myDictionary)
    {
        //
    }

***

### Local Functions

- Small functions defined inside another
- Faster than lambdas - no allocations
- Improved readability

***

### Timely argument validation!

- Iterators, async methods won't throw until first yield/await
- Fixed with local functions

*** 

### Example - argument validation

    [lang=cs]
        IEnumerable<int> MyMethod(int count)
        {
            if (count<0) throw new ArgumentException(nameof(count));

            IEnumerable<int> doCount()
            {
                for(int i=0;i<count;i++)
                {
                	yield return someComplexCalculation(i);
                }
            }

            return doCount();
        }

***

### Functional Dependency injection 

- Store a lastProcessed file. How to test
- Interface DI - IFileRead,IFileExist. YUCK!
- Why not pass the functions themselves?
- Could do it before, easier now
- How hard could it be?

***

### The injectionable class

    [lang=cs]
        public static class Cutoff
        {
            //Load the stored cutoff value, MinDate otherwise
            public static DateTime LoadFrom(string cutoffFile,Func<string,bool> exists=null,Func<string,string> read=null)
            {
                exists = exists ?? File.Exists;
                read = read ?? File.ReadAllText;

                if (!exists(cutoffFile)) return DateTime.MinValue;

                //out variable - The only C# 7 specific feature here!
                DateTime.TryParse(read(cutoffFile), out DateTime cutoff1);
                return cutoff1;
            }

            //Store the current time as a cutoff value.
            public static void StoreTo(string cutoffFile,Action<string,string> write=null)
            {
                write = write ?? File.WriteAllText;
                write(cutoffFile, DateTime.UtcNow.ToString("u"));
            }

        }    

***

### The tests

    [lang=cs]
        [Test]
        public void CanLoadFromExistingFile()
        {
            var expected=new DateTime(2017,4,1);

            var result=Cutoff.LoadFrom("existingFile", exists => true, read => "2017-04-01");

            Assert.That(result,Is.EqualTo(expected));

        }        

        [Test]
        public void MissingFileReturnsMinDate()
        {
            var result = Cutoff.LoadFrom("noFile", exists => false);

            Assert.That(result, Is.EqualTo(DateTime.MinValue));

        }        

***

### Benefits

- Adheres to the interfaces non-proliferation treaty ![Nuke](images\nuke.jpg)
- No need for a DI container to test
- Very clean tests
- Default behaviour 

***

### Memoization

Take any function and cache intermediate results

	[lang=CS]
	public Func<TIn,TOut> Memoize<TIn,TOut>(Func<TIn,TOut> f)
	{
		var cache=new Dictionary<TIn,TOUt>();
		TOut do(TIn x) 
		{
	        if (cache.ContainsKey(x)) return cache[x];
	        else 
	        {
	            var result=f(x);
	            cache[x]=result;
	            return result;
	        }
	    }

	    return do;
	}

***

### Pattern Matching and The Guns of Navarone

![Gun](images\Gun.jpg)

*** 

### What are they?

- Test a value against a "shape" - values or types
- Not complete but the revolution is coming
- Better than overrides or Visitor

***

### Nick Craver is Happy (and Dapper too)

    Cast and type check at once
    https://twitter.com/Nick_Craver/status/861258820991037440

    From 
    [lang=cs]
        public static bool Update<T>(this IDbConnection connection...)
        {
            var proxy= entityToUpdate as IProxy;
            if (proxy!-null)
            {
                if (!proxy.IsDirty) return false;
            }
            //Now start working
        }
        
    [lang=cs]

        public static bool Update<T>(this IDbConnection connection...)
        {
            if (entityToUpdate is IProxy proxy && !proxy.IsDirty)
            {
                return false;
            }
            //Now start working
        }

***

### Getting rid of type checking

Circles, boxes, yada yada

    [lang=cs]
        switch(shape)
        {
            case Circle c:
                WriteLine($"circle with radius {c.Radius}");
                break;
            case Rectangle s when (s.Length == s.Height):
                WriteLine($"{s.Length} x {s.Height} square");
                break;
            case Rectangle r:
                WriteLine($"{r.Length} x {r.Height} rectangle");
                break;
            default:
                WriteLine("<unknown shape>");
                break;
            case null:
                throw new ArgumentNullException(nameof(shape));
        }

***

### How about some *real* gains?

From 

    [lang=cs]
        private static bool TryStringSplit(ref IEnumerable list,int splitAt,string namePrefix,IDbCommand command, bool byPosition)
        {
            if (list==null || splitAt<0) return false;
            //Have to specify the types explicitly
            if (list is IEnumerable<int>) return TryStringSplit<int>(ref list, splitAt,namePrefix, command,"int",byPostition,
                (sb,i) => sb.Append(i.ToString(CultureInfo.InvariantCulture)));
            if (list is IEnumerable<long>) return TryStringSplit<long>(ref list, splitAt,namePrefix, command,"long",byPostition,
                (sb,i) => sb.Append(i.ToString(CultureInfo.InvariantCulture)));        
            //Enough already !!
        }

***

### How about some *real* gains?

    To 
    [lang=cs]
        private static bool TryStringSplit(ref IEnumerable list,int splitAt,string namePrefix,IDbCommand command, bool byPosition)
        {
            if (list==null || splitAt<0) return false;
            switch(list)
            {
                //It's strongly typed!
                case IEnumerable<int> l:
                    //No need to specify a type!
                    return TryStringSplit(ref l, splitAt,namePrefix, command,"int",byPostition,
                        (sb,i) => sb.Append(i.ToString(CultureInfo.InvariantCulture)));
                case IEnumerable<long> l:
                    return TryStringSplit(ref l, splitAt,namePrefix, command,"long",byPostition,
                        (sb,i) => sb.Append(i.ToString(CultureInfo.InvariantCulture)));
            }            
        }



*** 

### Out varialbes

- Just saving a line
- And tightening the scope
- But when you put it together

***

### Nick Craver is happy again

From 

    [lang=cs]
        object SqlMapper.IParameterLookup.this[string name]
        {
            get
            {
                ParamInfo param;
                return parameters.TryGetValue(name,out param)?param.Value : null;
            }
        }
    
To

    [lang=cs]
        object SqlMapper.IParameterLookup.this[string name] =>
            parameters.TryGetValue(name,out ParamInfo param)?param.Value : null;
    
That's where local functions are born

***

### A whiff  of gunpowder

- New error handling scenarios
- Exceptions are *Exceptional*
- Like a blown fuse, not a light switch
- Not a logging control flow statement

What about non-critical *errors*

![Fire](images/FuseFire.jpg)

***

### Go's multiple return values


    [lang=cs]
        (string result,string error) TheQuestion()
        {
            if (vogonsArrived)
            {
                return (null,"Goodbye and thanks for all the fish");
            }
            else 
            {
                return ("How much is 6 by 8?",null);
            }            
        }


        var (result,error)=TheQuestion();

***

### Caveats

![Fuses](images\Fuses.jpg)

***

### Caveats 

- Only `variable not used` warning if error ignored
- No warning if tuple isn't decomposed
- Take care to avoid the ignored error curse    
- Not for public APIs - names aren't preserved

*** 

### Pattern Matching and Case Types

- Scala feature
- One type, multiple cases
- No base type littering
- More Nick Craver happiness
- Result<TSuccess,TFailure> 


***

### Result Type

- Return a Result<TSuccess,TFailure> type 
- Allows composition of functions
- Works for public APIs


***

### Example

    [lang=cs]
        public interface IResult<TSuccess,TFailure>{}
        public class Success<TSuccess,TFailure>:IResult<TSuccess,TFailure>{
            public TSuccess Success {get;}

            Success(TSuccess it)=> Success=it;
        }
        public class Failure<TSuccess,TFailure>:IResult<TSuccess,TFailure>{
            public TFailure Error {get;}

            Success(TFailure it)=> Error=it;
        }

***

### Usage        

	[lang=CS]
    public IResult<int,string> DoSomething(){};

    public IResult<int,string> DoAnother()
    {
        var result=DoSomething();
        switch(result)
        {
            case Success<int,string> c: return new Success(-c.Success);
            case Failure<int,string> c: return new Failure(c.Error)
        }
    }

Return types can be chained


***

### Caveats

- No exhaustive matching
- Take care to avoid the ignored error curse


