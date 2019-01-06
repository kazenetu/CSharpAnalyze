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
    public LocalReference(ILocalReferenceOperation operation)
    {
      Expressions.Add(new Expression(operation.Local.Name, Expression.GetSymbolTypeName(operation.Local)));
    }
  }
}
