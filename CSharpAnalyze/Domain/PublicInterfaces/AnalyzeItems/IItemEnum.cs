using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:列挙型
  /// </summary>
  public interface IItemEnum : IAnalyzeItem
  {
    /// <summary>
    /// 列挙型の値リスト
    /// </summary>
    /// <remarks>名称と初期値のリスト</remarks>
    Dictionary<string, string> Items { get; }
  }
}
