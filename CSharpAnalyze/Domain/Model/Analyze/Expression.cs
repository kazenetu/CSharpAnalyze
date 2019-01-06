using CSharpAnalyze.Domain.PublicInterfaces;
using Microsoft.CodeAnalysis;
using System;

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
        return methodSymbol.MethodKind.ToString();
      }
      if (target is ILocalSymbol || target is IFieldSymbol || target is IPropertySymbol)
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

  }
}
