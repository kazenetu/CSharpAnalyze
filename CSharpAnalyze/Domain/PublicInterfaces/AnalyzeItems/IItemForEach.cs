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
    /// コレクション
    /// </summary>
    List<IExpression> Collection { get; }
  }
}
