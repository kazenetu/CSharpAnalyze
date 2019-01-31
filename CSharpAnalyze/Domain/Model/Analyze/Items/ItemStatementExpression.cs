using CSharpAnalyze.Domain.PublicInterfaces;
using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
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
  internal class ItemStatementExpression : AbstractItem, IItemStatementExpression
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
        case IIncrementOrDecrementOperation param:
          RightSideList.AddRange(OperationFactory.GetExpressionList(param.Target));
          switch (param.Kind)
          {
            case OperationKind.Increment:
              RightSideList.Add(new Expression("++", string.Empty));
              break;
            case OperationKind.Decrement:
              RightSideList.Add(new Expression("--", string.Empty));
              break;
          }
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

      // インデックススペースを作成
      result.Append(indexSpace);

      // 左辺の作成
      if (LeftSideList.Any())
      {
        result.Append(GetSideExpressionList(LeftSideList));
        result.Append(" = ");
      }

      // 右辺の作成
      if (RightSideList.Any())
      {
        result.Append(GetSideExpressionList(RightSideList));
      }
      result.Append(";");

      return result.ToString();
    }

    /// <summary>
    /// IExpressionのリストを元にC#のソースコードを再現する
    /// </summary>
    /// <param name="targetList">取得対象のリスト</param>
    /// <returns>再現したC#のソースコード</returns>
    private string GetSideExpressionList(List<IExpression> targetList)
    {
      var result = new StringBuilder();

      // スペースを置かないキーワード
      var nonSpaceKeywords = new List<string>() { ".", "(", ")", "[", "]","," };

      // リストから文字列を作成する
      for (var i = 0; i < targetList.Count; i++)
      {
        var isSetSpace = true;
        if (i == targetList.Count - 1)
        {
          isSetSpace = false;
        }
        else if (nonSpaceKeywords.Contains(targetList[i].Name) || nonSpaceKeywords.Contains(targetList[i + 1].Name))
        {
          isSetSpace = false;
        }
        result.Append($"{targetList[i].Name}");
        if (isSetSpace)
        {
          result.Append(" ");
        }
      }

      return result.ToString();
    }

    #endregion
  }
}
