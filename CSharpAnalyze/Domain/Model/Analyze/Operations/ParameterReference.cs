using Microsoft.CodeAnalysis.Operations;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:ParameterReference
  /// </summary>
  internal class ParameterReference : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    public ParameterReference(IParameterReferenceOperation operation)
    {
      var symbol = operation.Parameter;
      Expressions.Add(new Expression(symbol.Name, Expression.GetSymbolTypeName(symbol)));
    }
  }
}
