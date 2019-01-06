using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

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
      if (operation.Field.IsStatic)
      {
        Expressions.Add(new Expression(operation.Field.ContainingSymbol.Name,
                    Expression.GetSymbolTypeName(operation.Field.ContainingSymbol)));
        Expressions.Add(new Expression(".", string.Empty));

        // 外部参照チェック
        if (operation.Field.ContainingSymbol is INamedTypeSymbol)
        {
          // 外部ファイル参照イベント発行
          RaiseEvents.RaiseOtherFileReferenced(operation.Syntax, operation.Field.ContainingSymbol);
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
