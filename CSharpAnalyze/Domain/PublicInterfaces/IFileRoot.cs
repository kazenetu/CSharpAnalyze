using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces
{
  /// <summary>
  /// ファイルルート インターフェース
  /// </summary>
  public interface IFileRoot
  {
    /// <summary>
    /// ファイルパス
    /// </summary>
    string FilePath { get; }

    /// <summary>
    /// 外部参照のクラス名とファイルパスのリスト
    /// </summary>
    Dictionary<string, string> OtherFiles { get; }

    /// <summary>
    /// 子メンバ
    /// </summary>
    List<IAnalyzeItem> Members { get; }
  }
}
