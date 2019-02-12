using CSharpAnalyze.ApplicationService;
using CSharpAnalyze.Domain.Event;
using CSharpAnalyze.Domain.PublicInterfaces.Events;
using System;
using System.Globalization;
using System.Text;

namespace CSharpAnalyzeTest.Common
{
  /// <summary>
  /// テストのスーパークラス
  /// </summary>
  public class TestBase : IDisposable
  {
    /// <summary>
    /// テスト対象
    /// </summary>
    protected AnalyzeApplication CSAnalyze = new AnalyzeApplication();

    /// <summary>
    /// テスト用FileRepository
    /// </summary>
    protected CSFileRepositoryMock Files = new CSFileRepositoryMock();

    /// <summary>
    /// ソースコード
    /// </summary>
    private string BaseSource = string.Empty;

    /// <summary>
    /// Setup
    /// </summary>
    public TestBase()
    {
      // 基本ソースの組み立て
      var baseSource = new StringBuilder();
      baseSource.AppendLine("using System;");
      baseSource.AppendLine("using System.Collections.Generic;");
      baseSource.AppendLine("{0}");
      baseSource.AppendLine("namespace CSharpAnalyzeTest{{");
      baseSource.AppendLine("{1}");
      baseSource.AppendLine("}}");
      BaseSource = baseSource.ToString();

    }

    /// <summary>
    /// Teardown
    /// </summary>
    public void Dispose()
    {
      EventContainer.Unregister<IAnalyzed>(CSAnalyze);

      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// テスト情報作成
    /// </summary>
    /// <param name="fileName">ファイル名</param>
    /// <param name="addUsing">追加Using</param>
    /// <param name="sourceCode">ソースコード</param>
    protected void CreateFileData(string fileName,string addUsing, string sourceCode)
    {
      var source = string.Format(CultureInfo.CurrentCulture, BaseSource, addUsing, sourceCode);
      Files.Add(fileName, source);
    }
  }
}
