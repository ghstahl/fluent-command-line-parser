Fluent Command Line Parser
==========================
A simple, strongly typed .NET C# command line parser library using a fluent easy to use interface.

### Differences between this variant and the original
1. All features of the original
2. No distinction made when it comes to option switch reserved characters: -,--,/ are all equal
2. No distinction made between short and long option names, expect for stacking, where stacking is only honored for option names of lenght=1
3. stacking: -a -b, can be writen as -ab, --ab, and /ab.  -Dog -a cannot be stacked, i.e -Doga
4. The parser honors both case sensitive and case insensitive option names.  i.e. you can have both for any option.

Hopefully "https://github.com/siywilliams/fluent-command-line-parser", will pick up these changes and I will shut down this version.

### Download

See what's new in [v1.4.2](https://github.com/fclp/fluent-command-line-parser/wiki/Roadmap).

You can use the Package Manager console in Visual Studio:
```
PM> Install-Package Pingo.FluentCommandLineParser
```

### Usage
See [here](https://github.com/fclp/fluent-command-line-parser/wiki/So,-how-does-FCLP-compare-to-other-parsers%3F) for a side-by-side syntax comparison between other command line parsers.

Commands such as `updaterecord.exe -r 10 -v="Mr. Smith" --silent` can be captured using
```
static void Main(string[] args)
{

      var helpOption = p.Setup<bool>(CaseType.CaseInsensitive, helpOptions)
                .Callback(value => helpSwitch = value)
                .SetDefault(false);

  var p = new FluentCommandLineParser();

  p.Setup<int>(CaseType.CaseInsensitive,"r")
   .Callback(record => RecordID = record)
   .Required();

  p.Setup<string>(CaseType.CaseInsensitive,"v")
   .Callback(value => NewValue = value)
   .Required();

  p.Setup<bool>(CaseType.CaseInsensitive,"s", "silent")
   .Callback(silent => InSilentMode = silent)
   .SetDefault(false);

  p.Parse(args);
}
```
### Parser Option Methods

`.Setup<int>(CaseType.CaseInsensitive,"r")` Setup an option using a single name, 

`.Setup<int>(CaseType.CaseInsensitive,"r", "record")` or any number of names.

`.AddCaseSensitiveOption("BigR")` You can add a case sensitive name to an option as well.

`.AddCaseInsensitiveOption("WhAtEvEr")` You can add a case Insensitive name to an option as well.

`.Required()` Indicate the option is required and an error should be raised if it is not provided.

`.Callback(val => Value = val)` Provide a delegate to call after the option has been parsed

`.SetDefault(int.MaxValue)` Define a default value if the option was not specified in the args

`.WithDescription("Execute operation in silent mode without feedback")` Specify a help description for the option

### Parsing Using the Generic Fluent Command Line Parser

Instead of assigning parsed values to variables you can use the generic Fluent Command Line Parser to automatically create a defined object type and setup individual Options for each strongly-typed property. Because the generic parser is simply a wrapper around the standard fluent parser you can still use the Fluent Command Line Parser Api to define the behaviour for each Option.

The generic Fluent Command Line Parser can build a type and populate the properties with parsed values such as in the following example: 
```
public class ApplicationArguments
{
   public int RecordId { get; set; }
   public bool Silent { get; set; }
   public string NewValue { get; set; }
}

static void Main(string[] args)
{
   // create a generic parser for the ApplicationArguments type
   var p = new FluentCommandLineParser<ApplicationArguments>();

   // specify which property the value will be assigned too.
   p.Setup(arg => arg.RecordId)
    .As('r', "record") // define the short and long option name
    .Required(); // using the standard fluent Api to declare this Option as required.

   p.Setup(arg => arg.NewValue)
    .As('v', "value")
    .Required();

   p.Setup(arg => arg.Silent)
    .As('s', "silent")
    .SetDefault(false); // use the standard fluent Api to define a default value if non is specified in the arguments

   var result = p.Parse(args);

   if(result.HasErrors == false)
   {
      // use the instantiated ApplicationArguments object from the Object property on the parser.
      application.Run(p.Object);
   }
}
```

### Parsing To Collections

Many arguments can be collected as part of a list. Types supported are `string`, `int`, `double`, `bool` and `Enum`

For example arguments such as

`--filenames C:\file1.txt C:\file2.txt "C:\other file.txt"`

can be automatically parsed to a `List<string>` using
```
static void Main(string[] args)
{
   var p = new FluentCommandLineParser();

   var filenames = new List<string>();

   p.Setup<List<string>>('f', "filenames")
    .Callback(items => filenames = items);

   p.Parse(args);

   Console.WriteLine("Input file names:");

   foreach (var filename in filenames)
   {
      Console.WriteLine(filename);
   }
}
```
output:
```
Input file names
C:\file1.txt
C:\file2.txt
C:\other file.txt
```
### Enum support
Since v1.2.3 enum types are now supported. 
```
[Flags]
enum Direction
{
    North = 1,
    East = 2,
    South = 4,
    West = 8,
}
```
```
p.Setup<Direction>("direction")
 .Callback(d => direction = d);
```
To specify 'East' direction either the text can be provided or the enum integer.
```
dosomething.exe --direction East
dosomething.exe --direction 2
```

You can also collect multiple Enum values into a List<TEnum>
```
List<Direction> direction;

p.Setup<List<Direction>>('d', "direction")
 .Callback(d => direction = d);
```
For example, specifiying 'South' and 'East' values
```
dosomething.exe --direction South East
dosomething.exe --direction 4 2
```

Since v1.4 Enum Flags are also supported
```
Direction direction;

p.Setup<Direction>("direction")
 .Callback(d => direction = d);

p.Parse(args);

Assert.IsFalse(direction.HasFlag(Direction.North));
Assert.IsTrue(direction.HasFlag(Direction.East));
Assert.IsTrue(direction.HasFlag(Direction.South));
Assert.IsFalse(direction.HasFlag(Direction.West));
```

And the generic FluentCommandLineParser<T> (previously known as FluentCommandLineBuilder) also supports enums.

```
public class Args
{
   public Direction Direction { get;set; }
   public List<Direction> Directions { get;set; }
}
```
```
var p = new FluentCommandLineParser<Args>();

p.Setup(args => args.Direction)
 .As('d', "direction");

p.Setup(args => args.Directions)
 .As("directions");
```

### Help Screen
You can setup any help arguments, such as -? or --help to print all parameters which have been setup, along with their descriptions to the console by using SetupHelp(params string[]).

For example:

    // sets up the parser to execute the callback when -? or --help is detected
    parser.SetupHelp("?", "help")
     .Callback(text => Console.WriteLine(text));

Since v1.4.1 you can also choose to display the formatted help screen text manually, so that you can display it under other circumstances.


For example:

    var parser = new FluentCommandLineParser<Args>();
    
    parser.SetupHelp("?", "help")
     .Callback(text => Console.WriteLine(text));
    
    // triggers the SetupHelp Callback which writes the text to the console
    parser.HelpOption.ShowHelp(parser.Options);



### Supported Syntax
`[-|--|/][switch_name][=|:| ][value]`

Supports boolean names
```
example.exe -s  // enable
example.exe -s- // disabled
example.exe -s+ // enable
```
Supports combined (grouped) options
```
example.exe -xyz  // enable option x, y and z
example.exe -xyz- // disable option x, y and z
example.exe -xyz+ // enable option x, y and z
```
### Development
Fclp is in the early stages of development. Please feel free to provide any feedback on feature support or the Api itself.

If you would like to contribute, you may do so to the [develop branch](https://github.com/fclp/fluent-command-line-parser/tree/develop).

[![githalytics.com alpha](https://cruel-carlota.pagodabox.com/cbcae8086524a79bd8779e37b579a244 "githalytics.com")](http://githalytics.com/fclp/fluent-command-line-parser)
