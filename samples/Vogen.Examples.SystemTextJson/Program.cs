using System.Reflection;
using Vogen.Examples.SystemTextJson.SystemTextJsonExamples;

try
{
    Console.WriteLine($"{Assembly.GetEntryAssembly()?.FullName} examples:");

    new NonStrict().Run();
    new Strict().Run();
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