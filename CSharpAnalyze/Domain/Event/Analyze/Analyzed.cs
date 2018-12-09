using CSharpAnalyze.Domain.Model.Analyze;
using CSharpAnalyze.Domain.Model.Analyze.Items;

namespace CSharpAnalyze.Domain.Event.Analyze
{
  /// <summary>
  /// 解析完了イベント
  /// </summary>
  public class Analyzed : IEvent
  {
    public string FilePath { get; }
    public IAnalyzeItem AnalyzeResult { get; } = null;
    public IFileRoot FileRoot { get; } = null;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="filepath">ファイルパス</param>
    /// <param name="analyzeResult">解析結果</param>
    public Analyzed(string filepath, IAnalyzeItem analyzeResult)
    {
      FilePath = filepath;
      AnalyzeResult = analyzeResult;
    }

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
