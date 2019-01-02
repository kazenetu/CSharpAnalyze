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
    public Literal(ILiteralOperation operation)
    {
      var literalValue = operation.ConstantValue.Value;
      if (literalValue is string)
      {
        literalValue = $"\"{literalValue}\"";
      }
      Expressions.Add(new Expression(literalValue.ToString(), literalValue.GetType().Name));

    }
  }
}
