using CSharpAnalyze.Domain.Event;
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
    /// <param name="container">イベントコンテナ</param>
    /// <returns>IExpressionリスト</returns>
    public static List<IExpression> GetExpressionList(IOperation operation, EventContainer container)
    {
      List<IExpression> result = new List<IExpression>();

      AbstractOperation instance = null;
      switch (operation)
      {
        // 式系
        case ISimpleAssignmentOperation param:
          instance = new SimpleAssignment(param, container);
          break;
        case IInvocationOperation param:
          instance = new Invocation(param, container);
          break;
        case IBinaryOperation param:
          instance = new Binary(param, container);
          break;
        case IIncrementOrDecrementOperation param:
          instance = new Increment(param, container);
          break;
        case ICompoundAssignmentOperation param:
          instance = new CompoundAssignment(param, container);
          break;
        case IConversionOperation param:
          result.AddRange(GetExpressionList(param.Operand, container));
          break;

        // 参照系
        case IInstanceReferenceOperation param:
          instance = new InstanceReference(param, container);
          break;
        case IFieldReferenceOperation param:
          instance = new FieldReference(param, container);
          break;
        case IPropertyReferenceOperation param:
          instance = new PropertyReference(param, container);
          break;
        case ILocalReferenceOperation param:
          instance = new LocalReference(param, container);
          break;
        case IParameterReferenceOperation param:
          instance = new ParameterReference(param, container);
          break;
        case IArrayElementReferenceOperation param:
          instance = new ArrayElementReference(param, container);
          break;
        case IArgumentOperation param:
          instance = new Argument(param, container);
          break;

        // 生成系
        case IObjectCreationOperation param:
          instance = new ObjectCreation(param, container);
          break;
        case IArrayCreationOperation param:
          instance = new ArrayCreation(param, container);
          break;
        case IVariableDeclaratorOperation param:
          instance = new VariableDeclarator(param, container);
          break;

        // 直値
        case ILiteralOperation param:
          instance = new Literal(param, container);
          break;

        // Switch Case系
        case IDefaultCaseClauseOperation  param:
          instance = new DefalutCase(param, container);
          break;
        case IDeclarationPatternOperation param:
          instance = new DeclarationPattern(param, container);
          break;

        //If系
        case IIsPatternOperation param:
        instance = new IsPattern(param, container);
          break;
        case IIsTypeOperation param:
          instance = new IsType(param, container);
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
