using Microsoft.CodeAnalysis.Operations;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:PropertyReference
  /// </summary>
  internal class PropertyReference : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    public PropertyReference(IPropertyReferenceOperation operation)
    {
      if (operation.Instance is ILocalReferenceOperation local)
      {
        Expressions.Add(new Expression(local.Local.Name, local.Local.Type.Name));
        Expressions.Add(new Expression(".", string.Empty));
      }
      else
      {
        Expressions.Add(new Expression(operation.Property.ContainingSymbol.Name, operation.Property.ContainingSymbol.Kind.ToString()));
        Expressions.Add(new Expression(".", string.Empty));
      }

      Expressions.Add(new Expression(operation.Property.Name, operation.Property.Type.Name));
    }
  }
}
