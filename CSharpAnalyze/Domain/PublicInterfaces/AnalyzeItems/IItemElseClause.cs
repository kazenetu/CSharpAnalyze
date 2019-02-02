using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:ElseClause
  /// </summary>
  public interface IItemElseClause : IAnalyzeItem
  {
    /// <summary>
    /// 条件
    /// </summary>
    List<IExpression> Conditions { get; }

    /// <summary>
    /// 内部処理
    /// </summary>
    List<IAnalyzeItem> Block { get; }
  }
}
