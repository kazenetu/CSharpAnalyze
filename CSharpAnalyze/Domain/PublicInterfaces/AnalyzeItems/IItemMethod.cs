using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:メソッド
  /// </summary>
  public interface IItemMethod : IAnalyzeItem
  {
    /// <summary>
    /// メソッドの型リスト
    /// </summary>
    List<IExpression> MethodTypes { get; }

    /// <summary>
    /// パラメーターリスト
    /// </summary>
    List<(string name, List<IExpression> expressions, List<string> modifiers, List<IExpression> defaultValues)> Args { get; }

    /// <summary>
    /// ジェネリックタイプリスト
    /// </summary>
    List<string> GenericTypes { get; }
  }
}
