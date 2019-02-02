using Microsoft.CodeAnalysis.Operations;

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
    public Binary(IBinaryOperation operation)
    {
      // 左辺
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.LeftOperand));

      // 演算子
      Expressions.AddRange(Expression.GetOperationKindExpression(operation));

      // 右辺
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.RightOperand));
    }
  }
}
