using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:SwitchCase
  /// </summary>
  public interface IItemSwitchCase : IAnalyzeItem
  {
    /// <summary>
    /// Caseラベルリスト
    /// </summary>
    List<List<IExpression>> Labels { get; }
  }
}
