using CSharpAnalyze.Domain.Event;
using CSharpAnalyze.Domain.Event.Analyze;
using CSharpAnalyze.Domain.PublicInterfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// Item系クラスのスーパークラス
  /// </summary>
  internal abstract class AbstractItem: IAnalyzeItem
  {
    #region 基本インターフェース実装：プロパティ

    /// <summary>
    /// 親情報
    /// </summary>
    public IAnalyzeItem Parent { get; }

    /// <summary>
    /// 子メンバ
    /// </summary>
    public List<IAnalyzeItem> Members { get; } = new List<IAnalyzeItem>();

    /// <summary>
    /// アイテム種別
    /// </summary>
    public ItemTypes ItemType { get; protected set; }

    /// <summary>
    /// 名前
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// 修飾子リスト
    /// </summary>
    public List<string> Modifiers { get; } = new List<string>();

    /// <summary>
    /// コメント
    /// </summary>
    public List<string> Comments { get; } = new List<string>();

    #endregion

    #region 基本インターフェース実装：メソッド

    /// <summary>
    /// 文字列取得
    /// </summary>
    /// <param name="index">前スペース数</param>
    /// <returns>文字列</returns>
    public abstract string ToString(int index = 0);

    #endregion

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="parent">親IAnalyzeItem</param>
    /// <param name="node">対象Node</param>
    /// <param name="target">対象ソースのsemanticModel</param>
    protected AbstractItem(IAnalyzeItem parent,SyntaxNode node, SemanticModel semanticModel)
    {
      // 親インスタンスを設定
      Parent = parent;

      // 名前設定
      Name = node.DescendantTokens().
          Where(token => token.IsKind(SyntaxKind.IdentifierToken)).
          Select(token => semanticModel.GetDeclaredSymbol(token.Parent)).
          Where(symbol => symbol != null).FirstOrDefault()?.Name;

      // 識別子リスト設定
      var modifiersObject = node.GetType().GetProperty("Modifiers")?.GetValue(node);
      if(modifiersObject is SyntaxTokenList modifiers)
      {
        Modifiers.AddRange(modifiers.Select(item => item.Text));
      }

      // コメント設定
      var targerComments = node.GetLeadingTrivia().ToString().Split(Environment.NewLine).
                            Select(item => item.TrimStart().Replace(Environment.NewLine, string.Empty, StringComparison.CurrentCulture)).
                            Where(item => !string.IsNullOrEmpty(item));
      Comments.AddRange(targerComments);
    }

    /// <summary>
    /// シンボルインターフェースの型の名前を返す
    /// </summary>
    /// <param name="target">対象シンボルインターフェース</param>
    /// <returns>型名・存在しない場合はstring.Empty</returns>
    protected string GetSymbolTypeName(ISymbol target)
    {
      var methodSymbol = target as IMethodSymbol;
      if (methodSymbol != null)
      {
        return methodSymbol.MethodKind.ToString();
      }
      var localSymboll = target as ILocalSymbol;
      if (localSymboll != null)
      {
        return localSymboll.Kind.ToString();
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
    /// 外部ファイル参照イベント発行
    /// </summary>
    /// <param name="targetNode">対象Node</param>
    /// <param name="targetSymbol">比較対象のSymbol</param>
    protected void RaiseOtherFileReferenced(SyntaxNode targetNode, ISymbol targetSymbol)
    {
      if (!targetSymbol.DeclaringSyntaxReferences.Any())
      {
        // ファイルパスなしでイベント送信
        EventContainer.Raise(new OtherFileReferenced(string.Empty, targetSymbol.Name));
      }

      var targetNodeFilePath = targetNode.SyntaxTree.FilePath;
      var ReferenceFilePaths = targetSymbol.DeclaringSyntaxReferences.Select(item => item.SyntaxTree.FilePath).Where(filePath => filePath != targetNodeFilePath);
      foreach (var referenceFilePath in ReferenceFilePaths)
      {
        // ファイルパスありでイベント送信
        EventContainer.Raise(new OtherFileReferenced(referenceFilePath, targetSymbol.Name));
      }
    }
  }
}
