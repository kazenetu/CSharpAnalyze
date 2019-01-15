using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System.Linq;

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
        if(operation.Instance is ILocalReferenceOperation)
        {
          // ローカルの場合はクリアする
          Expressions.Clear();
        }
        else
        {
          // ローカル以外の場合はピリオドを設定
          Expressions.Add(new Expression(".", string.Empty));
        }

        // プロパティ格納
        SetProperty(operation);
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

        // プロパティ格納
        SetProperty(prop);
      }
      else
      {
        Expressions.AddRange(OperationFactory.GetExpressionList(operation));
      }

      return true;
    }

    /// <summary>
    /// プロパティ格納
    /// </summary>
    /// <param name="prop">親プロパティ</param>
    private void SetProperty(IPropertyReferenceOperation prop)
    {
      if (prop.Property is IPropertySymbol propItem)
      {
        if (propItem.IsIndexer)
        {
          // インデクサの場合
          Expressions.AddRange(OperationFactory.GetExpressionList(prop.Instance));
          Expressions.Add(new Expression("[", string.Empty));
          foreach (var arg in prop.Arguments)
          {
            if (!arg.Equals(prop.Arguments.First()))
            {
              Expressions.Add(new Expression(",", string.Empty));
            }
            Expressions.Add(new Expression(arg.Parameter.Name, Expression.GetSymbolTypeName(arg.Parameter.Type)));
          }
          Expressions.Add(new Expression("]", string.Empty));
        }
        else
        {
          // それ以外のプロパティの場合
          Expressions.Add(new Expression(propItem.Name, Expression.GetSymbolTypeName(propItem)));
        }
      }
      else
      {
        // プロパティ以外の場合
        Expressions.Add(new Expression(prop.Property.Name, Expression.GetSymbolTypeName(prop.Property)));
      }
    }
  }
}
