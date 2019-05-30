using CSharpAnalyze.Domain.Event;
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
    /// <param name="container">イベントコンテナ</param>
    public Invocation(IInvocationOperation operation, EventContainer container) : base(container)
    {
      if(operation.Instance is null)
      {
        // 組み込み・クラスメンバ
        var nameSpace = $"{operation.TargetMethod.ContainingNamespace.Name}.";
        var className = $"{operation.TargetMethod.ContainingType}".Replace(nameSpace, string.Empty, System.StringComparison.CurrentCulture);
        Expressions.Add(new Expression(className, string.Empty));
      }
      else
      {
        // インスタンス
        Expressions.AddRange(OperationFactory.GetExpressionList(operation.Instance, container));
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
        Expressions.AddRange(OperationFactory.GetExpressionList(arg.Value, container));

        isFirst = false;
      }
      Expressions.Add(new Expression(")", string.Empty));
    }
  }
}
