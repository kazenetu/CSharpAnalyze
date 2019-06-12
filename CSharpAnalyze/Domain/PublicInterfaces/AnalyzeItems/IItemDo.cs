using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:繰り返し do while
  /// </summary>
  public interface IItemDo : IAnalyzeItem
  {
    /// <summary>
    /// 条件
    /// </summary>
    List<IExpression> Conditions { get; }
  }
}
