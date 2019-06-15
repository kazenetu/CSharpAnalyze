using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:foreach
  /// </summary>
  public interface IItemForEach : IAnalyzeItem
  {
    /// <summary>
    /// ローカル
    /// </summary>
    List<IExpression> Local { get; }

    /// <summary>
    /// ローカルが型推論か否か
    /// </summary>
    bool IsVar { get; }

    /// <summary>
    /// ローカルの型リスト
    /// </summary>
    List<IExpression> LocalTypes { get; }

    /// <summary>
    /// コレクション
    /// </summary>
    List<IExpression> Collection { get; }

    /// <summary>
    /// コレクションの型リスト
    /// </summary>
    List<IExpression> CollectionTypes { get; }
  }
}
