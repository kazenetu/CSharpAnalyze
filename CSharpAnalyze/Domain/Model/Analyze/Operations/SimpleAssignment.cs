using Microsoft.CodeAnalysis.Operations;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:SimpleAssignment
  /// </summary>
  internal class SimpleAssignment : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    public SimpleAssignment(ISimpleAssignmentOperation operation)
    {
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.Target));
      Expressions.Add(new Expression("=",string.Empty));
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.Value));
    }
  }
}
