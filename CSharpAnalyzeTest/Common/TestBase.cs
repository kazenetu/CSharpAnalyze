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

      // 解析終了時に呼ばれるイベントを登録
      EventContainer.Register<IAnalyzed>(CSAnalyze, (ev) =>
      {
        var method = Files.GetDelegateMethod(ev.FilePath);
        if (method == null) return;

        method(ev);
      });
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
    /// <param name="delegateMethod">イベント処理</param>
    protected void CreateFileData(string fileName,string addUsing, string sourceCode, Action<IAnalyzed> delegateMethod)
    {
      var source = string.Format(CultureInfo.CurrentCulture, BaseSource, addUsing, sourceCode, delegateMethod);
      Files.Add(fileName, source, delegateMethod);
    }

    /// <summary>
    /// テスト情報作成
    /// </summary>
    /// <param name="fileData">テスト情報作成</param>
    /// <param name="delegateMethod">イベント処理</param>
    protected void CreateFileData(FileData fileData, Action<IAnalyzed> delegateMethod)
    {
      var source = string.Format(CultureInfo.CurrentCulture, BaseSource, fileData.AddUsing, fileData.Source, delegateMethod);
      Files.Add(fileData.FilePath, source, delegateMethod);
    }
  }
}
