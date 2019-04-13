using CSharpAnalyze.Domain.Event;
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
    /// <param name="container">イベントコンテナ</param>
    public SimpleAssignment(ISimpleAssignmentOperation operation, EventContainer container) : base(container)
    {
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.Target, container));
      Expressions.Add(new Expression("=",string.Empty));
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.Value, container));
    }
  }
}
