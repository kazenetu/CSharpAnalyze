using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis.Operations;
using System.Globalization;

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

      // nullの場合はその情報を格納して終了
      if (literalValue is null)
      {
        var type = "null";
        Expressions.Add(new Expression(type, type));
        return;
      }

      // 各Typeごとの加工処理
      if (literalValue is string)
      {
        literalValue = $"\"{literalValue}\"";
      }
      if (literalValue is bool)
      {
        literalValue = $"{literalValue}".ToLower(CultureInfo.CurrentCulture);
      }

      // 情報格納
      Expressions.Add(new Expression(literalValue.ToString(), Expression.GetSymbolTypeName(operation.Type)));
    }
  }
}
