using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:プロパティ
  /// </summary>
  public interface IItemProperty : IAnalyzeItem
  {
    /// <summary>
    /// プロパティの型リスト
    /// </summary>
    List<IExpression> PropertyTypes { get; }

    /// <summary>
    /// デフォルト設定リスト
    /// </summary>
    List<IExpression> DefaultValues { get; }

    /// <summary>
    /// アクセサリスト
    /// </summary>
    List<IAnalyzeItem> AccessorList { get; }
  }
}
