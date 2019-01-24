using CSharpAnalyze.Domain.Model.Analyze.Operations;
using CSharpAnalyze.Domain.PublicInterfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;

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
        // 式系
        case ISimpleAssignmentOperation param:
          instance = new SimpleAssignment(param);
          break;
        case IInvocationOperation param:
          instance = new Invocation(param);
          break;
        case IBinaryOperation param:
          instance = new Binary(param);
          break;
        case IIncrementOrDecrementOperation param:
          instance = new Increment(param);
          break;
        case ICompoundAssignmentOperation param:
          instance = new CompoundAssignment(param);
          break;

        // 参照系
        case IInstanceReferenceOperation param:
          instance = new InstanceReference(param);
          break;
        case IFieldReferenceOperation param:
          instance = new FieldReference(param);
          break;
        case IPropertyReferenceOperation param:
          instance = new PropertyReference(param);
          break;
        case ILocalReferenceOperation param:
          instance = new LocalReference(param);
          break;
        case IParameterReferenceOperation param:
          instance = new ParameterReference(param);
          break;
        case IArrayElementReferenceOperation param:
          instance = new ArrayElementReference(param);
          break;
        case IArgumentOperation param:
          instance = new Argument(param);
          break;

        // 生成系
        case IObjectCreationOperation param:
          instance = new ObjectCreation(param);
          break;
        case IArrayCreationOperation param:
          instance = new ArrayCreation(param);
          break;
        case IVariableDeclaratorOperation param:
          instance = new VariableDeclarator(param);
          break;

        // 直値
        case ILiteralOperation param:
          instance = new Literal(param);
          break;

        // その他
        case IDefaultCaseClauseOperation  param:
          instance = new DefalutCase(param);
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
