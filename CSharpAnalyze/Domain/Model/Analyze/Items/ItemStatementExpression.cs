using CSharpAnalyze.Domain.PublicInterfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// アイテム：式
  /// </summary>
  internal class ItemStatementExpression : AbstractItem
  {
    /// <summary>
    /// 左辺リスト
    /// </summary>
    /// <remarks>代入式以外はCount=0</remarks>
    public List<IExpression> LeftSideList { get; } = new List<IExpression>();

    /// <summary>
    /// 左辺リスト
    /// </summary>
    public List<IExpression> RightSideList { get; } = new List<IExpression>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="target">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    public ItemStatementExpression(ExpressionStatementSyntax node, SemanticModel semanticModel, IAnalyzeItem parent) : base(parent, node, semanticModel)
    {
      ItemType = ItemTypes.MethodStatement;

      // 式情報を取得
      var operation = semanticModel.GetOperation(node.Expression);
      switch (operation)
      {
        case ISimpleAssignmentOperation param:
          LeftSideList.AddRange(OperationFactory.GetExpressionList(param.Target));
          RightSideList.AddRange(OperationFactory.GetExpressionList(param.Value));
          break;
        case IInvocationOperation param:
          RightSideList.AddRange(OperationFactory.GetExpressionList(param));
          break;
        case IPropertyReferenceOperation param:
          RightSideList.AddRange(OperationFactory.GetExpressionList(param));
          break;
        case ILocalReferenceOperation param:
          RightSideList.AddRange(OperationFactory.GetExpressionList(param));
          break;
        case ILiteralOperation param:
          RightSideList.AddRange(OperationFactory.GetExpressionList(param));
          break;
        case IFieldReferenceOperation param:
          RightSideList.AddRange(OperationFactory.GetExpressionList(param));
          break;
        case IInstanceReferenceOperation param:
          RightSideList.AddRange(OperationFactory.GetExpressionList(param));
          break;
        case ICompoundAssignmentOperation param:
          LeftSideList.AddRange(OperationFactory.GetExpressionList(param.Target));
          RightSideList.AddRange(OperationFactory.GetExpressionList(param.Value));
          break;
        default:
          Console.Write($" [{operation.Kind} is none] ");
          break;
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

      result.Append(indexSpace);
      if (LeftSideList.Any())
      {
        foreach(var leftItem in LeftSideList)
        {
          result.Append(leftItem.Name);
        }
        result.Append(" = ");
      }
      if (RightSideList.Any())
      {
        foreach (var rightItem in RightSideList)
        {
          result.Append(rightItem.Name);
        }
      }
      result.AppendLine(";");

      //foreach (var comment in Comments)
      //{
      //  result.Append(indexSpace);
      //  result.AppendLine($"{comment}");
      //}

      //foreach (var modifier in Modifiers)
      //{
      //  result.Append(indexSpace);
      //  result.Append($"{modifier} ");
      //}
      //result.Append(indexSpace);

      //// プロパティの型
      //Types.ForEach(type => result.Append(type.Name));

      //// プロパティ名
      //result.Append($" {Name}");

      //if (DefaultValues.Any())
      //{
      //  // デフォルト値
      //  result.Append(" = ");

      //  foreach (var value in DefaultValues)
      //  {
      //    result.Append($"{value.Name}");
      //    if (value.Name == "new")
      //    {
      //      result.Append(" ");
      //    }
      //  }
      //  result.Append(";");
      //}

      return result.ToString();
    }

    #endregion
  }
}
