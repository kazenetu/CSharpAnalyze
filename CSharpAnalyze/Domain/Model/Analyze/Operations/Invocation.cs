using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:Invocation
  /// </summary>
  internal class Invocation : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    public Invocation(IInvocationOperation operation)
    {
      if(operation.Instance is null)
      {
        // 組み込み
        //Expressions.Add(new Expression(operation.Type.Name,Expression.GetSymbolTypeName(operation.Type)));
        Expressions.Add(new Expression($"{operation.TargetMethod.ConstructedFrom.ContainingType}", string.Empty));
      }
      else
      {
        // インスタンス
        Expressions.AddRange(OperationFactory.GetExpressionList(operation.Instance));
      }
      Expressions.Add(new Expression(".", string.Empty));

      // メソッドがローカルメソッドの場合はクリアする
      if(operation.TargetMethod.MethodKind == MethodKind.LocalFunction)
      {
        Expressions.Clear();
      }

      // メソッド名
      Expressions.Add(new Expression(operation.TargetMethod.Name, operation.TargetMethod.MethodKind.ToString()));

      // メソッドパラメータ
      Expressions.Add(new Expression("(", string.Empty));
      var isFirst = true;
      foreach (var arg in operation.Arguments)
      {
        if (!isFirst)
        {
          Expressions.Add(new Expression(",", string.Empty));
        }
        Expressions.AddRange(OperationFactory.GetExpressionList(arg.Value));

        isFirst = false;
      }
      Expressions.Add(new Expression(")", string.Empty));
    }
  }
}
