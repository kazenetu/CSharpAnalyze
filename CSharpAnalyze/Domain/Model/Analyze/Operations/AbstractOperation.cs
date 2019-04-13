using CSharpAnalyze.Domain.Event;
using CSharpAnalyze.Domain.Event.Analyze;
using CSharpAnalyze.Domain.PublicInterfaces;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation系のスーパークラス
  /// </summary>
  internal abstract class AbstractOperation
  {
    /// <summary>
    /// コンテナ
    /// </summary>
    protected EventContainer eventContainer = null;

    /// <summary>
    /// Expression ValueObject
    /// </summary>
    public List<IExpression> Expressions { get; } = new List<IExpression>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="container">イベントコンテナ</param>
    public AbstractOperation(EventContainer container)
    {
      // イベントコンテナ設定
      eventContainer = container;
    }

    /// <summary>
    /// 外部ファイル参照イベント発行
    /// </summary>
    /// <param name="targetNode">対象Node</param>
    /// <param name="targetSymbol">比較対象のSymbol</param>
    protected void RaiseOtherFileReferenced(SyntaxNode targetNode, ISymbol targetSymbol)
    {
      if (!targetSymbol.DeclaringSyntaxReferences.Any())
      {
        // ファイルパスなしでイベント送信
        eventContainer.Raise(new OtherFileReferenced(string.Empty, targetSymbol.Name));
        return;
      }

      var targetNodeFilePath = targetNode.SyntaxTree.FilePath;
      var ReferenceFilePaths = targetSymbol.DeclaringSyntaxReferences.Select(item => item.SyntaxTree.FilePath).Where(filePath => filePath != targetNodeFilePath);
      foreach (var referenceFilePath in ReferenceFilePaths)
      {
        // ファイルパスありでイベント送信
        eventContainer.Raise(new OtherFileReferenced(referenceFilePath, targetSymbol.Name));
      }
    }
  }
}
