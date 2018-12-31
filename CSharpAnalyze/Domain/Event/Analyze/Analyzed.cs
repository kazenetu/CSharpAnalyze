using CSharpAnalyze.Domain.PublicInterfaces;
using CSharpAnalyze.Domain.PublicInterfaces.Events;

namespace CSharpAnalyze.Domain.Event.Analyze.Events
{
  /// <summary>
  /// 解析完了イベント
  /// </summary>
  internal class Analyzed : IAnalyzed
  {
    public string FilePath { get; }
    public IFileRoot FileRoot { get; } = null;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="fileRoot">解析結果のファイル情報</param>
    public Analyzed(IFileRoot fileRoot)
    {
      FilePath = fileRoot.FilePath;
      FileRoot = fileRoot;
    }
  }
}
