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
    /// <param name="container">イベントコンテナ</param>
    public PropertyReference(IPropertyReferenceOperation operation, EventContainer container) : base(container)
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
            RaiseOtherFileReferenced(prop.Syntax, prop.Property.ContainingSymbol);
          }
        }

        // インスタンスプロパティの場合はthisを追加する
        if(prop.Instance is IInstanceReferenceOperation)
        {
          Expressions.AddRange(OperationFactory.GetExpressionList(prop.Instance, eventContainer));
          Expressions.Add(new Expression(".", string.Empty));
        }

        // プロパティ格納
        SetProperty(prop);
      }
      else
      {
        Expressions.AddRange(OperationFactory.GetExpressionList(operation, eventContainer));
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
          Expressions.AddRange(OperationFactory.GetExpressionList(prop.Instance, eventContainer));
          Expressions.Add(new Expression("[", string.Empty));
          foreach (var arg in prop.Arguments)
          {
            if (!arg.Equals(prop.Arguments.First()))
            {
              Expressions.Add(new Expression(",", string.Empty));
            }
            Expressions.AddRange(OperationFactory.GetExpressionList(arg, eventContainer));

          }
          Expressions.Add(new Expression("]", string.Empty));
        }
        else
        {
          // ローカルの場合はインスタンスを追加する
          if (prop.Instance is ILocalReferenceOperation)
          {
            Expressions.AddRange(OperationFactory.GetExpressionList(prop.Instance, eventContainer));
            Expressions.Add(new Expression(".", string.Empty));
          }

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
