using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:式
  /// </summary>
  public interface IItemStatementExpression : IAnalyzeItem
  {
    /// <summary>
    /// 左辺リスト
    /// </summary>
    /// <remarks>代入式以外はCount=0</remarks>
    List<IExpression> LeftSideList { get; }

    /// <summary>
    /// 左辺リスト
    /// </summary>
    List<IExpression> RightSideList { get; }
  }
}
