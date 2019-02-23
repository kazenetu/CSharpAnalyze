using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:for
  /// </summary>
  public interface IItemFor : IAnalyzeItem
  {
    /// <summary>
    /// 宣言部
    /// </summary>
    List<List<IExpression>> Declarations { get; }

    /// <summary>
    /// 宣言部が型推論か否か
    /// </summary>
    /// <remarks>定義済の場合は設定なし</remarks>
    bool IsVar { get; }

    /// <summary>
    /// 宣言部の型リスト
    /// </summary>
    /// <remarks>定義済の場合は設定なし</remarks>
    List<IExpression> Types { get; }

    /// <summary>
    /// 条件部
    /// </summary>
    List<IExpression> Conditions { get; }

    /// <summary>
    /// 計算部
    /// </summary>
    List<List<IExpression>> Incrementors { get; }
  }
}
