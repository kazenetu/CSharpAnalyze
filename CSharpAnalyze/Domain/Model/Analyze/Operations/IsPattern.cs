using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis.Operations;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:IsPattern
  /// </summary>
  internal class IsPattern : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    /// <param name="container">イベントコンテナ</param>
    public IsPattern(IIsPatternOperation operation, EventContainer container) : base(container)
    {
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.Value, container));
      Expressions.Add(new Expression("is", ""));
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.Pattern, container));
    }
  }
}
