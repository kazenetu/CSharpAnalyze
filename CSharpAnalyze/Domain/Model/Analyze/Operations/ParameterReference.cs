using CSharpAnalyze.Domain.Event;
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
    /// <param name="container">イベントコンテナ</param>
    public ParameterReference(IParameterReferenceOperation operation, EventContainer container) : base(container)
    {
      var symbol = operation.Parameter;
      Expressions.Add(new Expression(symbol.Name, Expression.GetSymbolTypeName(symbol)));
    }
  }
}
