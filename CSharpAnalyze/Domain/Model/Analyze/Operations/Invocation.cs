using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:SimpleAssignment
  /// </summary>
  internal class Invocation : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    public Invocation(IInvocationOperation operation)
    {
      // インスタンス
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.Instance));
      Expressions.Add(new Expression(".", string.Empty));

      // メソッド名
      Expressions.Add(new Expression(operation.TargetMethod.Name, operation.TargetMethod.GetType().ToString()));

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
