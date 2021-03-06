﻿using CSharpAnalyze.Domain.Event;
using CSharpAnalyze.Domain.PublicInterfaces;
using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// アイテム：IF
  /// </summary>
  internal class ItemIf : AbstractItem, IItemIf
  {
    /// <summary>
    /// 条件
    /// </summary>
    public List<IExpression> Conditions { get; } = new List<IExpression>();

    /// <summary>
    /// 条件に一致した場合の処理リスト
    /// </summary>
    public List<IAnalyzeItem> TrueBlock { get; } = new List<IAnalyzeItem>();

    /// <summary>
    /// 条件に一致しない場合の処理リスト
    /// </summary>
    public List<IAnalyzeItem> FalseBlocks { get; } = new List<IAnalyzeItem>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="semanticModel">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    /// <param name="container">イベントコンテナ</param>
    public ItemIf(IfStatementSyntax node, SemanticModel semanticModel, IAnalyzeItem parent, EventContainer container) : base(parent, node, semanticModel, container)
    {
      ItemType = ItemTypes.MethodStatement;

      var condition = semanticModel.GetOperation(node.Condition);
      Conditions.AddRange(OperationFactory.GetExpressionList(condition, container));

      TrueBlock.AddRange(GetBlock(node.Statement, semanticModel));
      FalseBlocks.AddRange(GetElseBlock(node.Else));

      List<IAnalyzeItem> GetElseBlock(ElseClauseSyntax elseNode)
      {
        var result = new List<IAnalyzeItem>();

        if(elseNode is null)
        {
          return result;
        }

        result.Add(ItemFactory.Create(elseNode, semanticModel, container, parent));

        // else ifの場合はさらに続ける
        if (elseNode.Statement is IfStatementSyntax ifNode)
        {
          result.AddRange(GetElseBlock(ifNode.Else));
        }

        return result;
      }
    }

    /// <summary>
    /// ブロック内処理を取得
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="semanticModel)">対象ソースのsemanticModel</param>
    /// <returns>ブロック内処理(nullの場合は要素なし)</returns>
    private List<IAnalyzeItem> GetBlock(SyntaxNode node, SemanticModel semanticModel)
    {
      var result = new List<IAnalyzeItem>();

      BlockSyntax block = node as BlockSyntax;
      if (node is ElseClauseSyntax)
      {
        block = ((ElseClauseSyntax)node).Statement as BlockSyntax;
      }
      if(block == null)
      {
        return result;
      }

      foreach (var statement in block.Statements)
      {
        result.Add(ItemFactory.Create(statement, semanticModel, eventContainer, this));
      }

      return result;
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
      result.Append("if(");
      for(var i=0;i< Conditions.Count; i++)
      {
        var isSetSpace = true;
        if(i == Conditions.Count - 1)
        {
          isSetSpace = false;
        }
        else if(Conditions[i].Name == "." || Conditions[i+1].Name == ".")
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

      result.Append(indexSpace);
      result.AppendLine("{");
      foreach (var statement in TrueBlock)
      {
        result.AppendLine(statement.ToString(index + 1));
      }
      result.Append(indexSpace);
      result.AppendLine("}");

      if (FalseBlocks.Any())
      {
        foreach (var block in FalseBlocks)
        {
          result.Append(block.ToString(index));
        }
      }

      return result.ToString();
    }

    #endregion
  }
}
