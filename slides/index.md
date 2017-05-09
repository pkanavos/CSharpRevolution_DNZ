- title : C# 7 - Prelude to the Revolustion
- description :# 7 - Prelude to the Revolustion
- author : Panagiotis Kanavos
- theme : night
- transition : default


Lots of "small" changes but some are pretty big

On the road to Expressions
throw is now an expression

Value Tuples - Lazy or Crazy?

***

### Decomposition - Where are my keys?

    [lang=cs]
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
        Task MyMethod(sring somePath)
        {
            if (String.IsNullOrWhiteSpace(somePath)) throw new ArgumentException(nameof(somePath));

            async Task doX()
            {
                var result=await x.DoSomething();
            }

            return doX();
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

- Adheres to the interfaces non-proliferation treaty
- No need for a DI container to test
- Very clean tests
- Default behaviour 

***

###

Pattern Matching and The Guns of Navarone

Not complete but the revolution is coming
Better than overrides or Visitor

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

A sniff of gunpowder

New error handling scenarios

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

### Result Type

- Return a Result<TSuccess,TFailure> type 
- Allows composition of functions
- Works for public APIs


***

### Example

    [lang=cs]
        public interface IResult<TSuccess,TFailure>{}
        public class Success<TSuccess,TFailure>:IResult<TSuccess,TFailure>{
            public TSuccess {get;set;}
        }
        public class Failure<TSuccess,TFailure>:IResult<TSuccess,TFailure>{
            public TFailure {get;set;}
        }

        public IResult<int,string> DoSomething(){};

        public IResult<int,string> DoAnother()
        {
            var result=DoSomething();
            switch(result)
            {
                case Success<MyDTO,string> c: return new Success(-c);
                break;
            }
        }

***

### Return types can be chained


***

### Caveats

- No exhaustive matching
- Take care to avoid the ignored error curse

Exceptions are *Exceptional*
Like a blown fuse
Not a logging control flow statement

What about non-critical *errors*

