using CSharpAnalyze.Domain.PublicInterfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;

namespace CSharpAnalyze.Domain.Model.Analyze
{
  /// <summary>
  /// Expression ValueObject
  /// </summary>
  internal class Expression: IExpression
  {
    /// <summary>
    /// 名前
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 変数型名
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="name">名前</param>
    /// <param name="typeName">変数型名</param>
    public Expression(string name, string typeName)
    {
      Name = name;
      TypeName = typeName;

      // 組み込みは名前を変える
      switch (Name)
      {
        case nameof(Int32):
          Name = "int";
          break;
        case nameof(String):
          Name = "string";
          break;
      }
    }

    /// <summary>
    /// 比較
    /// </summary>
    /// <param name="other">比較対象</param>
    /// <returns>比較結果</returns>
    public bool Equals(IExpression other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Name == other.Name && TypeName == other.TypeName;
    }

    /// <summary>
    /// 比較
    /// </summary>
    /// <returns>比較結果</returns>
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((IExpression)obj);
    }

    /// <summary>
    /// ハッシュ値取得
    /// </summary>
    /// <returns>ハッシュ値</returns>
    public override int GetHashCode()
    {
      return Name.GetHashCode(StringComparison.CurrentCulture) + TypeName.GetHashCode(StringComparison.CurrentCulture);
    }

    /// <summary>
    /// シンボルインターフェースの型の名前を返す
    /// </summary>
    /// <param name="target">対象シンボルインターフェース</param>
    /// <returns>型名・存在しない場合はstring.Empty</returns>
    internal static string GetSymbolTypeName(ISymbol target)
    {
      var methodSymbol = target as IMethodSymbol;
      if (methodSymbol != null)
      {
        return methodSymbol.Kind.ToString();
      }
      if (target is ILocalSymbol || target is IFieldSymbol || 
          target is IPropertySymbol || target is IParameterSymbol)
      {
        return target.Kind.ToString();
      }

      var symbol = target as INamedTypeSymbol;
      if (symbol == null)
      {
        return string.Empty;
      }

      if (symbol.IsGenericType)
      {
        return "GenericClass";
      }

      if (symbol.SpecialType != SpecialType.None)
      {
        return symbol.Name;
      }

      return symbol.TypeKind.ToString();
    }

    /// <summary>
    /// 演算子用Expressionインスタンスを取得
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    /// <returns>Expressionインスタンス(Listで表現)</returns>
    internal static List<Expression> GetOperationKindExpression(IOperation operation)
    {
      var result = new List<Expression>();

      var operatorName = string.Empty;
      var operatorType = string.Empty;
      switch (operation)
      {
        case IBinaryOperation binaryOperation:
          switch (binaryOperation.OperatorKind)
          {
            case BinaryOperatorKind.Add:
              operatorName = "+";
              break;
            case BinaryOperatorKind.Subtract:
              operatorName = "-";
              break;
            case BinaryOperatorKind.Multiply:
              operatorName = "*";
              break;
            case BinaryOperatorKind.Divide:
              operatorName = "/";
              break;
            case BinaryOperatorKind.And:
              operatorName = "&&";
              break;
            case BinaryOperatorKind.ConditionalAnd:
              operatorName = "&";
              break;
            case BinaryOperatorKind.ConditionalOr:
              operatorName = "|";
              break;
            case BinaryOperatorKind.Equals:
              operatorName = "==";
              break;
            case BinaryOperatorKind.ExclusiveOr:
              operatorName = "^";
              break;
            case BinaryOperatorKind.GreaterThan:
              operatorName = ">";
              break;
            case BinaryOperatorKind.GreaterThanOrEqual:
              operatorName = ">=";
              break;
            case BinaryOperatorKind.LessThan:
              operatorName = "<";
              break;
            case BinaryOperatorKind.LessThanOrEqual:
              operatorName = "<=";
              break;
            case BinaryOperatorKind.NotEquals:
              operatorName = "!=";
              break;
            case BinaryOperatorKind.Or:
              operatorName = "||";
              break;
          }
          operatorType = binaryOperation.OperatorKind.ToString();
          break;
        default:
          switch (operation.Kind)
          {
            case OperationKind.Increment:
              operatorName = "++";
              break;
            case OperationKind.Decrement:
              operatorName = "--";
              break;
          }
          operatorType = operation.Kind.ToString();
          break;
      }
      if (!string.IsNullOrEmpty(operatorName))
      {
        result.Add(new Expression(operatorName, operatorType));
      }

      return result;
    }
  }
}
