using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:クラス
  /// </summary>
  public interface IItemClass : IAnalyzeItem
  {
    /// <summary>
    /// スーパークラスリスト
    /// </summary>
    List<IExpression> SuperClass { get; }

    /// <summary>
    /// インタフェースリスト
    /// </summary>
    List<List<IExpression>> Interfaces { get; }

    /// <summary>
    /// ジェネリックタイプリスト
    /// </summary>
    List<string> GenericTypes { get; }

    /// <summary>
    /// 継承元のフィールドリスト
    /// </summary>
    /// <remarks>参考情報</remarks>
    List<string> BaseFields { get; }

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
