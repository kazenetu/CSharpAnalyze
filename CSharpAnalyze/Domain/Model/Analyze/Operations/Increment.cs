using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis.Operations;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:IIncrementOrDecrement
  /// </summary>
  internal class Increment : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    /// <param name="container">イベントコンテナ</param>
    public Increment(IIncrementOrDecrementOperation operation, EventContainer container) : base(container)
    {
      // インクリメント・デクリメント対象インスタンス
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.Target, container));

      // インクリメント・デクリメント
      Expressions.AddRange(Expression.GetOperationKindExpression(operation));
    }
  }
}
