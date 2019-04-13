using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis.Operations;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:VariableDeclarator
  /// </summary>
  internal class VariableDeclarator : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    /// <param name="container">イベントコンテナ</param>
    public VariableDeclarator(IVariableDeclaratorOperation operation, EventContainer container) : base(container)
    {
      Expressions.Add(new Expression(operation.Symbol.Name,Expression.GetSymbolTypeName(operation.Symbol)));

      if (!(operation.Initializer is null))
      {
        Expressions.Add(new Expression("=", string.Empty));
        Expressions.AddRange(OperationFactory.GetExpressionList(operation.Initializer.Value, container));
      }
    }
  }
}
