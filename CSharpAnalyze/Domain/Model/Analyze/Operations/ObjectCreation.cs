using CSharpAnalyze.Domain.Event;
using CSharpAnalyze.Domain.Event.Analyze;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:ObjectCreation
  /// </summary>
  internal class ObjectCreation : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    public ObjectCreation(IObjectCreationOperation operation)
    {
      var node = operation.Syntax;
      var parts = operation.Type.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat);
      if (!parts.Any())
      {
        return;
      }

      // newキーワード追加
      Expressions.Add(new Expression("new", string.Empty));

      // クラス生成
      foreach (var part in parts)
      {
        var name = $"{part}";
        var type = GetSymbolTypeName(part.Symbol);
        if (part.Kind == SymbolDisplayPartKind.ClassName)
        {
          // 外部ファイル参照イベント発行
          RaiseOtherFileReferenced(node, part.Symbol);
        }

        Expressions.Add(new Expression(name, type));
      }

      // パラメーター取得
      Expressions.Add(new Expression("(", string.Empty));
      var isFirst = true;
      foreach(var param in operation.Arguments)
      {
        if (!isFirst)
        {
          Expressions.Add(new Expression(",", string.Empty));
        }
        Expressions.AddRange(OperationFactory.GetExpressionList(param));

        isFirst = false;
      }
      Expressions.Add(new Expression(")", string.Empty));
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
