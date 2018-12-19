using System;

namespace CSharpAnalyze.Domain.PublicInterfaces
{
  /// <summary>
  /// Expression ValueObject Interface
  /// </summary>
  public interface IExpression: IEquatable<IExpression>
  {
    /// <summary>
    /// 名前
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 変数型名
    /// </summary>
    string TypeName { get; }

    /// <summary>
    /// 比較
    /// </summary>
    /// <returns>比較結果</returns>
    bool Equals(object obj);

    /// <summary>
    /// ハッシュ値取得
    /// </summary>
    /// <returns>ハッシュ値</returns>
    int GetHashCode();
  }
}
