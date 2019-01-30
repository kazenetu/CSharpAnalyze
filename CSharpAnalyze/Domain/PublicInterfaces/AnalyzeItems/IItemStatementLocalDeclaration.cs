using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:ローカル変数
  /// </summary>
  public interface IItemStatementLocalDeclaration : IAnalyzeItem
  {
    /// <summary>
    /// 型リスト
    /// </summary>
    List<IExpression> Types { get; }

    /// <summary>
    /// デフォルト設定リスト
    /// </summary>
    List<IExpression> DefaultValues { get; }

    /// <summary>
    /// 型推論か否か
    /// </summary>
    bool IsVar { get; }
  }
}
