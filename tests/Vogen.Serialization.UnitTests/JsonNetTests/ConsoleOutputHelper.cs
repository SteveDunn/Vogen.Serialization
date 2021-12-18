using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace Vogen.Serialization.UnitTests.JsonNetTests;

public class ConsoleOutputHelper : TextWriter, ITestOutputHelper
{
    private readonly ITestOutputHelper _helper;

    public ConsoleOutputHelper(ITestOutputHelper helper) => _helper = helper;

    public override void WriteLine(string? message) => _helper.WriteLine(message);

    public override void WriteLine(string format, params object?[] args) => _helper.WriteLine(format, args);

    public override Encoding Encoding => new UTF8Encoding();
}