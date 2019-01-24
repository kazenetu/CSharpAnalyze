using Microsoft.CodeAnalysis.Operations;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:CompoundAssignment
  /// </summary>
  internal class CompoundAssignment : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    public CompoundAssignment(ICompoundAssignmentOperation operation)
    {
      // 対象インスタンス
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.Target));

      var operatorName = string.Empty;
      switch (operation.OperatorKind)
      {
        case BinaryOperatorKind.Add:
          operatorName = "+";
          break;
        case BinaryOperatorKind.Subtract:
          operatorName = "-";
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
        Expressions.Add(new Expression($"{operatorName}=", string.Empty));
      }

      // 値
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.Value));
    }
  }
}
