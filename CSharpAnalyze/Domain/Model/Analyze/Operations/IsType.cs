using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:IsType
  /// </summary>
  internal class IsType : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    /// <param name="container">イベントコンテナ</param>
    public IsType(IIsTypeOperation operation, EventContainer container) : base(container)
    {
      var name = Expression.GetSymbolName(operation.TypeOperand, true);
      var type = Expression.GetSymbolTypeName(operation.TypeOperand);
      if (operation.TypeOperand.TypeKind == TypeKind.Class)
      {
        // 外部ファイル参照イベント発行
        RaiseOtherFileReferenced(operation.Syntax, operation.TypeOperand);
      }

      Expressions.AddRange(OperationFactory.GetExpressionList(operation.ValueOperand, container));
      Expressions.Add(new Expression("is", ""));
      Expressions.Add(new Expression(name, type));
    }
  }
}
