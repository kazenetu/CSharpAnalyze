﻿using CSharpAnalyze.Domain.PublicInterfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// アイテム：foreach
  /// </summary>
  internal class ItemForEach : AbstractItem
  {
    /// <summary>
    /// ローカル
    /// </summary>
    private List<IExpression> Local = new List<IExpression>();

    /// <summary>
    /// コレクション
    /// </summary>
    private List<IExpression> Collection = new List<IExpression>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="target">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    public ItemForEach(ForEachStatementSyntax node, SemanticModel semanticModel, IAnalyzeItem parent) : base(parent, node, semanticModel)
    {
      ItemType = ItemTypes.MethodStatement;

      var oparetion = semanticModel.GetOperation(node) as IForEachLoopOperation;

      // ローカル設定
      var localSymbol = oparetion.Locals.First();
      Local.Add(new Expression(localSymbol.Name, Expression.GetSymbolTypeName(localSymbol)));

      //コレクション
      Collection.AddRange(OperationFactory.GetExpressionList(oparetion.Collection.Children.First()));

      // 内部処理設定
      var block = node.Statement as BlockSyntax;
      foreach (var statement in block.Statements)
      {
        Members.Add(ItemFactory.Create(statement, semanticModel, this));
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
      result.Append("foreach(");
      result.Append(Local.First().Name);
      result.Append(" in ");
      result.Append(Collection.First().Name);
      result.AppendLine(")");

      result.Append(indexSpace);
      result.AppendLine("{");
      foreach (var statement in Members)
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