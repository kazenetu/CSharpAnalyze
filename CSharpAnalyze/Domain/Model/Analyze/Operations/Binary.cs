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
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.LeftOperand));

      var operatorName = string.Empty;
      switch (operation.OperatorKind)
      {
        case BinaryOperatorKind.Add:
          operatorName = "+";
          break;
        case BinaryOperatorKind.Multiply:
          operatorName = "*";
          break;
        case BinaryOperatorKind.Divide:
          operatorName = "/";
          break;
        case BinaryOperatorKind.And:
          operatorName = "&&";
          break;
        case BinaryOperatorKind.ConditionalAnd:
          operatorName = "&";
          break;
        case BinaryOperatorKind.ConditionalOr:
          operatorName = "|";
          break;
        case BinaryOperatorKind.Equals:
          operatorName = "==";
          break;
        case BinaryOperatorKind.ExclusiveOr:
          operatorName = "^";
          break;
        case BinaryOperatorKind.GreaterThan:
          operatorName = ">";
          break;
        case BinaryOperatorKind.GreaterThanOrEqual:
          operatorName = ">=";
          break;
        case BinaryOperatorKind.LessThan:
          operatorName = "<";
          break;
        case BinaryOperatorKind.LessThanOrEqual:
          operatorName = "<=";
          break;
        case BinaryOperatorKind.NotEquals:
          operatorName = "!=";
          break;
        case BinaryOperatorKind.Or:
          operatorName = "||";
          break;
      }
      if (!string.IsNullOrEmpty(operatorName))
      {
        Expressions.Add(new Expression(operatorName, string.Empty));
      }

      Expressions.AddRange(OperationFactory.GetExpressionList(operation.RightOperand));
    }
  }
}
