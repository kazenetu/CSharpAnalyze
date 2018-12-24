using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System.Linq;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:FieldReference
  /// </summary>
  internal class FieldReference : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    /// <param name="target">対象ソースのsemanticModel</param>
    public FieldReference(IFieldReferenceOperation operation)
    {
      if (operation.Field.DeclaringSyntaxReferences.Any())
      {
        Expressions.Add(new Expression(operation.Field.ContainingSymbol.Name, operation.Field.ContainingSymbol.GetType().Name));
        Expressions.Add(new Expression(".", string.Empty));
        Expressions.Add(new Expression(operation.Field.Name, operation.Field.Type.Name));
      }
      else
      {
        Expressions.Add(new Expression(operation.Field.ContainingSymbol.Name, operation.Field.ContainingSymbol.GetType().Name));
        Expressions.Add(new Expression(".", string.Empty));
        Expressions.Add(new Expression(operation.Field.Name, operation.Field.Type.Name));
      }
    }
  }
}
