using CSharpAnalyze.Domain.PublicInterfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// アイテム：SwitchCase
  /// </summary>
  internal class ItemSwitchCase : AbstractItem
  {
    /// <summary>
    /// Caseラベルリスト
    /// </summary>
    public List<List<IExpression>> Labels { get; } = new List<List<IExpression>>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="target">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    public ItemSwitchCase(SwitchSectionSyntax node, SemanticModel semanticModel, IAnalyzeItem parent) : base(parent, node, semanticModel)
    {
      ItemType = ItemTypes.MethodStatement;

      var operation = semanticModel.GetOperation(node) as ISwitchCaseOperation;

      // Caseラベル設定
      foreach (var item in operation.Clauses.Where(item => !(item is IDefaultCaseClauseOperation)))
      {
        Labels.Add(OperationFactory.GetExpressionList(item.Children.First()));
      }
      foreach(var item in operation.Clauses.Where(item => item is IDefaultCaseClauseOperation))
      {
        // TODO defaultを扱えるようにする
        Labels.Add(OperationFactory.GetExpressionList(item));
      }

      // 内部処理設定
      foreach (var statement in node.Statements)
      {
        var item = ItemFactory.Create(statement, semanticModel, this);

        if(!(item is null))
        {
          Members.Add(ItemFactory.Create(statement, semanticModel, this));
        }
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

      // Caseラベル
      foreach(var label in Labels)
      {
        result.Append(indexSpace);
        result.Append("case ");

        for (var i = 0; i < label.Count; i++)
        {
          var isSetSpace = true;
          if (i == label.Count - 1)
          {
            isSetSpace = false;
          }
          else if (label[i].Name == "." || label[i + 1].Name == ".")
          {
            isSetSpace = false;
          }
          result.Append($"{label[i].Name}");
          if (isSetSpace)
          {
            result.Append(" ");
          }
        }

        result.AppendLine(":");
      }

      // 内部処理
      foreach (var statement in Members)
      {
        result.AppendLine(statement.ToString(index + 1));
      }

      return result.ToString();
    }

    #endregion
  }
}
