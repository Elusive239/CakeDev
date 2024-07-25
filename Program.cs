// namespace Cake;
using System.Text;
using static Cake.Util;
using Cake;

bool record = false;
bool test = false;
string inputFile = $"{Directory.GetCurrentDirectory()}\\Tests\\while.ck";

for (int i = 0; i < args.Length; i++)
{
	switch (args[i])
	{
		case "-t": test = true; break;
		case "-r": record = true; break;
		case "-i": inputFile = args[++i]; break;
		case "-h":
			Console.WriteLine("Runs whichever \'.ck\' file it can find first with no flags set, then exits.");
			Console.WriteLine("Flags:");
			Console.WriteLine("\t-h\tGives optional flag list.\n");
			Console.WriteLine("\t-i\tSets the \'.ck\' file to be interpreted.\n");
			Console.WriteLine("\t-t\tRun available tests in \'Tests\' directory and compare to previously recorded results.\n");
			Console.WriteLine("\t-r\tRun available tests in \'Tests\' directory and record results.\n");
			return;
		default: throw new Exception($"Invalid Argument \'{args[i]}\'");
	}
}

if (!File.Exists(inputFile))
{
	try
	{
		inputFile = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.ck", SearchOption.AllDirectories)[0];
	}
	catch
	{
		inputFile = string.Empty;
		INFO("No \'*.ck\' file in current directory.");
	}
}


TextWriter original = Console.Out;
TextWriter? writer = null;

if (record)
{
	string[] filePaths = Directory.GetFiles($"Tests", "*.ck");
	foreach (var item in filePaths)
	{
		string path = $"{item[..^3]}.test";
		writer = new StreamWriter(path);
		Console.SetOut(writer);
		Execute(File.ReadAllText(item));
		writer.Close();
	}
	Console.SetOut(original);
	INFO($"Finished recording {filePaths.Length} results.");
}
if (test)
{
	string[] filePaths = Directory.GetFiles($"Tests", "*.ck");
	foreach (var item in filePaths)
	{
		string testPath = $"{item[..^3]}.test";
		StringBuilder builder = new();
		writer = new StringWriter(builder);
		Console.SetOut(writer);
		Execute(File.ReadAllText(item));
		writer.Close();
		Console.SetOut(original);
		if (builder.ToString().Equals(File.ReadAllText(testPath)))
		{
			INFO($"{item} -> PASS");
		}
		else INFO($"{item} -> FAIL");
	}
	Console.SetOut(original);
	INFO($"Finished testing {filePaths.Length} files.");
	return;
}

INFO($"Reading {inputFile}...");
string input = File.ReadAllText(inputFile);
if (record)
{
	writer = new StreamWriter($"{inputFile[..^3]}.test");
	Console.SetOut(writer);
}
Execute(input);
if (writer != null)
{
	writer.Close();
	Console.SetOut(original);
}
INFO("Ending execution.");

static void Execute(string input)
{
	try{
		#region LINES_THAT_MATTER
			Lexer lexer = new();
			Parser parser = new(lexer.Tokenize(input).ToArray());
			Evaluator evaluator = new();
			Main main = parser.Parse();
			int exitCode = evaluator.EvaluateMain(main);
		#endregion
		INFO($"Exit Code: {exitCode}");
		INFO($"{evaluator}");
	}
	catch (Exception e)
	{
		INFO($"{e}");
	}
}
