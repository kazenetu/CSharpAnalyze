using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis.Operations;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:Argument
  /// </summary>
  internal class Argument : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    /// <param name="container">イベントコンテナ</param>
    public Argument(IArgumentOperation operation, EventContainer container) : base(container)
    {
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.Value, container));
    }
  }
}
