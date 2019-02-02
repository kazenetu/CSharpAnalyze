using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:if
  /// </summary>
  public interface IItemIf : IAnalyzeItem
  {
    /// <summary>
    /// 条件
    /// </summary>
    List<IExpression> Conditions { get; }

    /// <summary>
    /// 条件に一致した場合の処理リスト
    /// </summary>
    List<IAnalyzeItem> TrueBlock { get; }

    /// <summary>
    /// 条件に一致しない場合の処理リスト
    /// </summary>
    List<IAnalyzeItem> FalseBlocks { get; }
  }
}
