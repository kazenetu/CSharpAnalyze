using CSharpAnalyze.Domain.PublicInterfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// アイテム：continue
  /// </summary>
  internal class ItemContinue : AbstractItem
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="target">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    public ItemContinue(ContinueStatementSyntax node, SemanticModel semanticModel, IAnalyzeItem parent) : base(parent, node, semanticModel)
    {
      ItemType = ItemTypes.MethodStatement;
    }

    #region 基本インターフェース実装：メソッド

    /// <summary>
    /// 文字列取得
    /// </summary>
    /// <param name="index">前スペース数</param>
    /// <returns>文字列</returns>
    public override string ToString(int index = 0)
    {
      var result = new StringBuilder();
      var indexSpace = string.Concat(Enumerable.Repeat("  ", index));

      foreach (var comment in Comments)
      {
        result.Append(indexSpace);
        result.AppendLine($"{comment}");
      }

      foreach (var modifier in Modifiers)
      {
        result.Append(indexSpace);
        result.Append($"{modifier} ");
      }
      result.Append(indexSpace);

      // break設定
      result.Append("continue;");

      return result.ToString();
    }
    #endregion
  }
}
