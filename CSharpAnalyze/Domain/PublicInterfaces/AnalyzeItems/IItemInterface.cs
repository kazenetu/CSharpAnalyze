using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:インターフェース
  /// </summary>
  public interface IItemInterface : IAnalyzeItem
  {
    /// <summary>
    /// インタフェースリスト
    /// </summary>
    List<List<IExpression>> Interfaces { get; }
  }
}
