using System.Diagnostics;
using Xunit.Abstractions;

namespace Vogen.Serialization.UnitTests.JsonNetTests;

public class TraceOutputHelper : TextWriterTraceListener, ITestOutputHelper
{
    private readonly ITestOutputHelper _helper;

    public TraceOutputHelper(ITestOutputHelper helper) => _helper = helper;

    public override void WriteLine(string? message) => _helper.WriteLine(message);

    public void WriteLine(string format, params object[] args) => _helper.WriteLine(format, args);
}