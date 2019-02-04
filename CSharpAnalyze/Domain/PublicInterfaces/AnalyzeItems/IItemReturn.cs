using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:return
  /// </summary>
  public interface IItemReturn : IAnalyzeItem
  {
    /// <summary>
    /// 戻り値リスト
    /// </summary>
    List<IExpression> ReturnValue { get; }
  }
}
