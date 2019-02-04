using CSharpAnalyze.Domain.PublicInterfaces;
using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// アイテム：return
  /// </summary>
  internal class ItemReturn : AbstractItem, IItemReturn
  {
    /// <summary>
    /// 戻り値リスト
    /// </summary>
    public List<IExpression> ReturnValue { get; } = new List<IExpression>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="target">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    public ItemReturn(ReturnStatementSyntax node, SemanticModel semanticModel, IAnalyzeItem parent) : base(parent, node, semanticModel)
    {
      ItemType = ItemTypes.MethodStatement;

      var operation = semanticModel.GetOperation(node) as IReturnOperation;

      // 戻り値設定
      ReturnValue.AddRange(OperationFactory.GetExpressionList(operation.ReturnedValue));
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

      // スペースを置かないキーワード
      var nonSpaceKeywords = new List<string>() { ".", "(", ")", "[", "]" };

      // 戻り値の作成
      result.Append("return ");

      // リストから文字列を作成する
      for (var i = 0; i < ReturnValue.Count; i++)
      {
        var isSetSpace = true;
        if (i == ReturnValue.Count - 1)
        {
          isSetSpace = false;
        }
        else if (nonSpaceKeywords.Contains(ReturnValue[i].Name) || nonSpaceKeywords.Contains(ReturnValue[i + 1].Name))
        {
          isSetSpace = false;
        }
        result.Append($"{ReturnValue[i].Name}");
        if (isSetSpace)
        {
          result.Append(" ");
        }
      }
      result.Append(";");

      return result.ToString();
    }

    #endregion
  }
}
