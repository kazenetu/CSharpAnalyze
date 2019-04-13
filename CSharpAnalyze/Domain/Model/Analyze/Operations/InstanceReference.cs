using CSharpAnalyze.Domain.Event;
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
    /// <param name="container">イベントコンテナ</param>
    public InstanceReference(IInstanceReferenceOperation operation, EventContainer container) : base(container)
    {
      Expressions.Add(new Expression("this", Expression.GetSymbolTypeName(operation.Type)));
    }
  }
}
