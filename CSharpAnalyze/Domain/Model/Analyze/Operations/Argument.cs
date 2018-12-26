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
    public Argument(IArgumentOperation operation)
    {
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.Value));
    }
  }
}
