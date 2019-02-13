using CSharpAnalyze.Domain.Event;
using CSharpAnalyze.Domain.PublicInterfaces.Events;
using CSharpAnalyzeTest.Common;
using System;
using Xunit;

namespace CSharpAnalyzeTest
{
  public class ClassTest : TestBase
  {
    /// <summary>
    /// Setup
    /// </summary>
    public ClassTest():base()
    {
      System.Diagnostics.Debug.WriteLine($"Setup {Environment.CurrentDirectory}");
    }

    [Fact]
    public void Test()
    {
      CreateFileData("Test.cs", string.Empty, "class Test{}");

      EventContainer.Register<IAnalyzed>(CSAnalyze, (ev) =>
      {
        System.Diagnostics.Debug.WriteLine($"[{ev.FilePath}]");
        System.Diagnostics.Debug.WriteLine(ev.FileRoot?.ToString());

        Assert.True(ev.FilePath == "Test.cs", $"ファイルパスが異なります{ev.FilePath}");
      });
      
      CSAnalyze.Analyze(string.Empty, Files);
    }

  }
}
