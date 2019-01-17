using CSharpAnalyze.Domain.PublicInterfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// アイテム：ElseClause
  /// </summary>
  internal class ItemElseClause : AbstractItem
  {
    public List<IExpression> Conditions { get; } = new List<IExpression>();
    public List<IAnalyzeItem> Block { get; } = new List<IAnalyzeItem>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="target">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    public ItemElseClause(ElseClauseSyntax node, SemanticModel semanticModel, IAnalyzeItem parent) : base(parent, node, semanticModel)
    {
      ItemType = ItemTypes.MethodStatement;

      if (node.Statement is IfStatementSyntax ifNode)
      {
        var condition = semanticModel.GetOperation(ifNode.Condition);
        Conditions.AddRange(OperationFactory.GetExpressionList(condition));

        var block = ifNode.Statement as BlockSyntax;
        foreach (var statement in block.Statements)
        {
          Block.Add(ItemFactory.Create(statement, semanticModel, this));
        }
      }
      else
      {
        var block = node.Statement as BlockSyntax;
        foreach (var statement in block.Statements)
        {
          Block.Add(ItemFactory.Create(statement, semanticModel, this));
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

      result.Append(indexSpace);
      if (Conditions.Any())
      {
        result.Append("else if(");
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
      }
      else
      {
        result.AppendLine("else");
      }

      result.Append(indexSpace);
      result.AppendLine("{");
      foreach (var statement in Block)
      {
        result.AppendLine(statement.ToString(index + 1));
      }
      result.Append(indexSpace);
      result.AppendLine("}");

      return result.ToString();
    }

    #endregion
  }
}
