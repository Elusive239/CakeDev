**Cake**
===========

The C# based scripting language where you can have your cake and eat it too! (***Dies***)

This was made for fun (in C# .Net 7.0) but maybe I'll put it in a game one day!. Also yes i've been writing this
without git for a good while! Time to put it on git for real and get this finished!

Click [here](SYNTAX.md) to get a hopefully short and easy to understand overview of the syntax. Subject to modification, will likely break as Cake is fleshed out.

SUPPORTS
--------

- strings
- math operations
- boolean operations
- variables
- if, elif, and else
- while Loops
- functions (not C# native, yet)
- arrays (regular, arrays in arrays, etc)
- 'structs'
- assert (debug keyword that ends program early if condition fails)

INSTRUCTIONS
-------------
First, clone the project somewhere.

Next, Install the .NET SDK. I'm currently using version \`7.0.410\`, but 7.0 and above should work 
fine. Anything lower is probably not going to work at the moment.

Then, if you want to use it as is:
>1. create a file in the same directory as the cloned project. the filename should end in \`.ck\`.
>2. write you first script in the above file (look in [SYNTAX.MD](SYNTAX.MD) for guidance).
>3. open a terminal in this projects location, and use the command "dotnet run"
>4. enjoy!

If you want to use this in your own projects, however, then you will be interested in this bit 
of C# code:

```C#
string input = "CODE";
Lexer lexer = new();
Parser parser = new(lexer.Tokenize(input).ToArray());
Main main = parser.Parse();
Evaluator evaluator = new();
evaluator.EvaluateMain(main);
int exitCode = evaluator.exitCode;
```

the important parts of the above block are the last two lines.
1. \`evaluator.EvaluateMain(main);\`:
>This returns something called an "ITokenLiteral". This is just value of some kind, a number or 
>a string. this is the final output of our script! 
2. \`int exitCode = evaluator.exitCode;\`:
>This is how we get the exit code. if this value is ever not 0, an \`assert\` statement failed and
>we exited the program early.

TODO
----

[ ] Global Vars in Cake functions (@var)

[ ] Functions (C#)

[ ] Loops (For i)

[ ] Escape Characters in strings

[ ] Refactor \`Prpgram.cs\`, it needs some work

[ ] Support C# objects?
