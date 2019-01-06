using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis;
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
      // プロパティ参照情報を追加
      if (!AddExpressions(operation.Instance))
      {
        AddExpressions(operation);
      }
      else
      {
        Expressions.Add(new Expression(".", string.Empty));
        Expressions.Add(new Expression(operation.Property.Name, 
                            Expression.GetSymbolTypeName(operation.Property.Type)));
      }
    }

    /// <summary>
    /// プロパティ参照情報を追加する
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    /// <returns>追加した場合はtrue nullの場合はfalse</returns>
    private bool AddExpressions(IOperation operation)
    {
      // operationがnullの場合はfalseを返して終了
      if (operation == null)
      {
        return false;
      }

      // プロパティ参照情報の追加
      if (operation is IPropertyReferenceOperation prop)
      {
        if (prop.Property.IsStatic)
        {
          Expressions.Add(new Expression(prop.Property.ContainingSymbol.Name, 
                              Expression.GetSymbolTypeName(prop.Property.ContainingSymbol)));
          Expressions.Add(new Expression(".", string.Empty));

          // 外部参照チェック
          if (prop.Property.ContainingSymbol is INamedTypeSymbol)
          {
            // 外部ファイル参照イベント発行
            RaiseEvents.RaiseOtherFileReferenced(prop.Syntax, prop.Property.ContainingSymbol);
          }
        }
        else
        {
          Expressions.Add(new Expression("this", Expression.GetSymbolTypeName(prop.Instance.Type)));
          Expressions.Add(new Expression(".", string.Empty));
        }

        Expressions.Add(new Expression(prop.Property.Name, Expression.GetSymbolTypeName(prop.Type)));
      }
      else
      {
        Expressions.AddRange(OperationFactory.GetExpressionList(operation));
      }

      return true;
    }
  }
}
