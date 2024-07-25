namespace Cake;
public static class Util
{
	public static void INFO(string arg) => Console.WriteLine($"[\x1b[92mINFO\x1b[39m] {arg}");
	public static Exception ERROR(string arg) => new($"[\x1b[91mFAIL\x1b[39m] {arg}");
}