using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:クラス
  /// </summary>
  public interface IItemClass : IAnalyzeItem
  {
    /// <summary>
    /// スーパークラスリスト
    /// </summary>
    List<IExpression> SuperClass { get; }

    /// <summary>
    /// ジェネリックタイプリスト
    /// </summary>
    List<string> GenericTypes { get; }
  }
}
