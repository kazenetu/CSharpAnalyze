using Microsoft.CodeAnalysis;
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
    public Increment(IIncrementOrDecrementOperation operation)
    {
      // インクリメント・デクリメント対象インスタンス
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.Target));

      // インクリメント・デクリメント
      switch (operation.Kind)
      {
        case OperationKind.Increment:
          Expressions.Add(new Expression("++", operation.Kind.ToString()));
          break;
        case OperationKind.Decrement:
          Expressions.Add(new Expression("--", operation.Kind.ToString()));
          break;
      }
    }
  }
}
