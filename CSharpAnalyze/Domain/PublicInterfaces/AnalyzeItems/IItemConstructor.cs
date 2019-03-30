using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:コンストラクタ
  /// </summary>
  public interface IItemConstructor : IAnalyzeItem
  {
    /// <summary>
    /// パラメーターリスト
    /// </summary>
    List<(string name, List<IExpression> expressions, List<string> modifiers, List<IExpression> defaultValues)> Args { get; }

    /// <summary>
    /// ベースパラメーターリスト
    /// </summary>
    List<string> BaseArgs { get; }
  }
}
