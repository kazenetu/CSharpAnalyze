using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis.Operations;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:LocalReference
  /// </summary>
  internal class LocalReference : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    /// <param name="container">イベントコンテナ</param>
    public LocalReference(ILocalReferenceOperation operation, EventContainer container) : base(container)
    {
      Expressions.Add(new Expression(operation.Local.Name, Expression.GetSymbolTypeName(operation.Local)));
    }
  }
}
