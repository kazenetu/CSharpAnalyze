using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis.Operations;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:Literal
  /// </summary>
  internal class Literal : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    /// <param name="container">イベントコンテナ</param>
    public Literal(ILiteralOperation operation, EventContainer container) : base(container)
    {
      var literalValue = operation.ConstantValue.Value;
      if (literalValue is string)
      {
        literalValue = $"\"{literalValue}\"";
      }
      if(literalValue is null)
      {
        var type = "null";
        Expressions.Add(new Expression(type, type));
      }
      else
      {
        Expressions.Add(new Expression(literalValue.ToString(), Expression.GetSymbolTypeName(operation.Type)));
      }
    }
  }
}
