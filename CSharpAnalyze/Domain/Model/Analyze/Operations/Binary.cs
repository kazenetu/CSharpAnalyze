using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:Binary
  /// </summary>
  internal class Binary : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    /// <param name="container">イベントコンテナ</param>
    public Binary(IBinaryOperation operation, EventContainer container) : base(container)
    {
      // 左辺
      OpenParentheses(operation.LeftOperand);
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.LeftOperand, container));
      CloseParentheses(operation.LeftOperand);

      // 演算子
      Expressions.AddRange(Expression.GetOperationKindExpression(operation));

      // 右辺
      OpenParentheses(operation.RightOperand);
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.RightOperand, container));
      CloseParentheses(operation.RightOperand);
    }

    /// <summary>
    /// 開始括弧の確認と追加
    /// </summary>
    /// <param name="targetOpration">対象Oprationインスタンス</param>
    private void OpenParentheses(IOperation targetOpration)
    {
      if (targetOpration is IBinaryOperation && ExistsParentheses(targetOpration))
      {
        Expressions.Add(new Expression("(", string.Empty));
      }
    }

    /// <summary>
    /// 終了括弧の確認と追加
    /// </summary>
    /// <param name="targetOpration">対象Oprationインスタンス</param>
    private void CloseParentheses(IOperation targetOpration)
    {
      if (targetOpration is IBinaryOperation && ExistsParentheses(targetOpration))
      {
        Expressions.Add(new Expression(")", string.Empty));
      }
    }

    /// <summary>
    /// 括弧で囲まれているかを返す
    /// </summary>
    /// <param name="targetOpration">対象Oprationインスタンス</param>
    /// <returns>IBinaryOperationなおかつ括弧で囲まれている場合のみtrue</returns>
    private bool ExistsParentheses(IOperation targetOpration)
    {
      if (targetOpration is IBinaryOperation && targetOpration.Syntax.Parent != null)
      {
        return $"{targetOpration.Syntax.Parent}".StartsWith("(", StringComparison.CurrentCulture);
      }
      return false;
    }

  }
}
