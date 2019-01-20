using CSharpAnalyze.Domain.Event;
using CSharpAnalyze.Domain.PublicInterfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// アイテム：Switch
  /// </summary>
  internal class ItemSwitch : AbstractItem
  {
    /// <summary>
    /// 条件
    /// </summary>
    public List<IExpression> Conditions { get; } = new List<IExpression>();

    /// <summary>
    /// Caseリスト
    /// </summary>
    public List<IAnalyzeItem> Cases { get; } = new List<IAnalyzeItem>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="target">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    public ItemSwitch(SwitchStatementSyntax node, SemanticModel semanticModel, IAnalyzeItem parent) : base(parent, node, semanticModel)
    {
      ItemType = ItemTypes.MethodStatement;

      var operation = semanticModel.GetOperation(node) as ISwitchOperation;

      // 条件設定
      Conditions.AddRange(OperationFactory.GetExpressionList(operation.Value));

      // Caseリスト設定
      foreach(var item in operation.Cases)
      {
        Cases.Add(ItemFactory.Create(item.Syntax,semanticModel,this));
      }

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

      // 条件
      result.Append(indexSpace);
      result.Append("switch(");
      for (var i = 0; i < Conditions.Count; i++)
      {
        var isSetSpace = true;
        if (i == Conditions.Count - 1)
        {
          isSetSpace = false;
        }
        else if (Conditions[i].Name == "." || Conditions[i + 1].Name == ".")
        {
          isSetSpace = false;
        }
        result.Append($"{Conditions[i].Name}");
        if (isSetSpace)
        {
          result.Append(" ");
        }
      }
      result.AppendLine(")");

      // ラベル
      result.Append(indexSpace);
      result.AppendLine("{");
      foreach (var caseItem in Cases)
      {
        result.Append(caseItem.ToString(index + 1));
      }
      result.Append(indexSpace);
      result.AppendLine("}");

      return result.ToString();
    }

    #endregion
  }
}
