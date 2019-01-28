using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:フィールド
  /// </summary>
  public interface IItemField : IAnalyzeItem
  {
    /// <summary>
    /// フィールドの型リスト
    /// </summary>
    List<IExpression> FieldTypes { get; }

    /// <summary>
    /// デフォルト設定リスト
    /// </summary>
    List<IExpression> DefaultValues { get; }
  }
}
