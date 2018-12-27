using CSharpAnalyze.Domain.Model.Analyze.Operations;
using CSharpAnalyze.Domain.PublicInterfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze
{
  /// <summary>
  /// IExpressionリスト取得クラス
  /// </summary>
  internal static class OperationFactory
  {
    /// <summary>
    /// IExpressionリスト取得
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    /// <returns>IExpressionリスト</returns>
    public static List<IExpression> GetExpressionList(IOperation operation)
    {
      List<IExpression> result = new List<IExpression>();

      AbstractOperation instance = null;
      switch (operation)
      {
        case ISimpleAssignmentOperation param:
          instance = new SimpleAssignment(param);
          break;
        case IInvocationOperation param:
          instance = new Invocation(param);
          break;
        case IPropertyReferenceOperation param:
          instance = new PropertyReference(param);
          break;
        case ILocalReferenceOperation param:
          instance = new LocalReference(param);
          break;
        case ILiteralOperation param:
          instance = new Literal(param);
          break;
        case IFieldReferenceOperation param:
          instance = new FieldReference(param);
          break;
        case IInstanceReferenceOperation param:
          instance = new InstanceReference(param);
          break;
        case IObjectCreationOperation param:
          instance = new ObjectCreation(param);
          break;
        case IArgumentOperation param:
          instance = new Argument(param);
          break;
        default:
          Console.Write($" [{operation.Kind} is none] ");
          break;
      }

      // リスト追加
      if(instance != null)
      {
        result.AddRange(instance.Expressions);
      }

      return result;
    }
  }
}
