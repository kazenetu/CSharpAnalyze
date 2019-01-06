using Microsoft.CodeAnalysis.Operations;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:InstanceReference
  /// </summary>
  internal class InstanceReference : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    public InstanceReference(IInstanceReferenceOperation operation)
    {
      Expressions.Add(new Expression("this", Expression.GetSymbolTypeName(operation.Type)));
    }
  }
}
