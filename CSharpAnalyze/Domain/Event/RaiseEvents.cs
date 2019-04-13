using CSharpAnalyze.Domain.Event.Analyze;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace CSharpAnalyze.Domain.Event
{
  /// <summary>
  /// イベント発行クラス
  /// </summary>
  internal static class RaiseEvents
  {
    /// <summary>
    /// 外部ファイル参照イベント発行
    /// </summary>
    /// <param name="targetNode">対象Node</param>
    /// <param name="targetSymbol">比較対象のSymbol</param>
    [System.Obsolete("use AbstractItem#RaiseOtherFileReferenced")]
    public static void RaiseOtherFileReferenced(SyntaxNode targetNode, ISymbol targetSymbol)
    {
    }

  }
}
