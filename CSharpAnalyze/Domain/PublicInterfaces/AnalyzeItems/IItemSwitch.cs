using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:Switch
  /// </summary>
  public interface IItemSwitch : IAnalyzeItem
  {
    /// <summary>
    /// 条件
    /// </summary>
    List<IExpression> Conditions { get; }

    /// <summary>
    /// Caseリスト
    /// </summary>
    List<IAnalyzeItem> Cases { get; }
  }
}
