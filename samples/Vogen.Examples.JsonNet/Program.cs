using System.Reflection;
using Vogen.Examples.JsonNetExamples;

try
{
    Console.WriteLine($"{Assembly.GetEntryAssembly()?.FullName} examples:");

    new Strict().Run();
    new NonStrict().Run();
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("OK!");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Error.WriteLine(ex);
}
finally
{
    Console.ResetColor();
}

