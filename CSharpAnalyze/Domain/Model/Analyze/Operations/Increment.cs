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
      var incrementOrDecrement = Expression.GetOperationKindExpression(operation);

      if(!operation.IsPostfix){
        // インクリメント・デクリメント
        Expressions.AddRange(incrementOrDecrement);
      }

      // インクリメント・デクリメント対象インスタンス
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.Target, container));

      if (operation.IsPostfix)
      {
        // インクリメント・デクリメント
        Expressions.AddRange(incrementOrDecrement);
      }
    }
  }
}
