using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:インターフェース
  /// </summary>
  public interface IItemInterface : IAnalyzeItem
  {
    /// <summary>
    /// インタフェースリスト
    /// </summary>
    List<List<IExpression>> Interfaces { get; }

    /// <summary>
    /// ジェネリックタイプリスト
    /// </summary>
    List<string> GenericTypes { get; }

    /// <summary>
    /// 継承元のプロパティリスト
    /// </summary>
    /// <remarks>参考情報</remarks>
    List<string> BaseProperties { get; }

    /// <summary>
    /// 継承元のメソッドリスト
    /// </summary>
    /// <remarks>参考情報</remarks>
    List<string> BaseMethods { get; }
  }
}
