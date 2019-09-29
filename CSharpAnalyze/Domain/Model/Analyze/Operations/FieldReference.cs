using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System;

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
    /// <param name="container">イベントコンテナ</param>
    public FieldReference(IFieldReferenceOperation operation, EventContainer container) : base(container)
    {
      if (operation.Field.IsStatic)
      {
        var name = $"{operation.Field.ContainingSymbol}".Replace($"{operation.Field.ContainingNamespace}.", string.Empty, StringComparison.CurrentCulture);
        Expressions.Add(new Expression(name,
                    Expression.GetSymbolTypeName(operation.Field.ContainingSymbol)));
        Expressions.Add(new Expression(".", string.Empty));

        // 外部参照チェック
        if (operation.Field.ContainingSymbol is INamedTypeSymbol)
        {
          // 外部ファイル参照イベント発行
          RaiseOtherFileReferenced(operation.Syntax, operation.Field.ContainingSymbol);
        }
      }
      else
      {
        Expressions.Add(new Expression("this", Expression.GetSymbolTypeName(operation.Field)));
        Expressions.Add(new Expression(".", string.Empty));
      }
      Expressions.Add(new Expression(operation.Field.Name, Expression.GetSymbolTypeName(operation.Field.Type)));
    }
  }
}
