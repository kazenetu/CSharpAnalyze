using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:ローカルメソッド
  /// </summary>
  public interface IItemLocalFunction : IAnalyzeItem
  {
    /// <summary>
    /// メソッドの型リスト
    /// </summary>
    List<IExpression> MethodTypes { get; }

    /// <summary>
    /// パラメーターリスト
    /// </summary>
    List<(string name, List<IExpression> expressions)> Args { get; }
  }
}
