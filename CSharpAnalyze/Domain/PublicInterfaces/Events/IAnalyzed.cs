namespace CSharpAnalyze.Domain.PublicInterfaces.Events
{
  /// <summary>
  /// 解析完了イベント インターフェース
  /// </summary>
  public interface IAnalyzed : IEvent
  {
    /// <summary>
    /// ファイル名
    /// </summary>
    string FilePath { get; }

    /// <summary>
    /// 解析情報ルート
    /// </summary>
    IFileRoot FileRoot { get; }
  }
}
