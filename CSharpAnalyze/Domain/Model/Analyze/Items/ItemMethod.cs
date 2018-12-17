using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// アイテム：メソッド
  /// </summary>
  internal class ItemMethod : AbstractItem
  {
    /// <summary>
    /// メソッドの型リスト
    /// </summary>
    public List<Expression> MethodTypes { get; } = new List<Expression>();

    /// <summary>
    /// パラメーターリスト
    /// </summary>
    public List<(string name, List<Expression> expressions)> Args { get; } = new List<(string name, List<Expression> expressions)>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="target">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    public ItemMethod(MethodDeclarationSyntax node, SemanticModel semanticModel, IAnalyzeItem parent) : base(parent, node, semanticModel)
    {
      ItemType = ItemTypes.Method;

      var declaredSymbol = semanticModel.GetDeclaredSymbol(node);

      // メソッドの型設定
      var parts = declaredSymbol.ReturnType.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat);
      foreach (var part in parts)
      {
        var name = $"{part}";
        var type = GetSymbolTypeName(part.Symbol);
        if (part.Kind == SymbolDisplayPartKind.ClassName)
        {
          // 外部ファイル参照イベント発行
          RaiseOtherFileReferenced(node, part.Symbol);
        }

        MethodTypes.Add(new Expression(name, type));
      }

      // パラメーター取得
      foreach(var param in declaredSymbol.Parameters)
      {
        var arg = new List<Expression>();

        var argParts = param.Type.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat);
        foreach (var part in argParts)
        {
          var name = $"{part}";
          var type = GetSymbolTypeName(part.Symbol);
          if (part.Kind == SymbolDisplayPartKind.ClassName)
          {
            // 外部ファイル参照イベント発行
            RaiseOtherFileReferenced(node, part.Symbol);
          }

          arg.Add(new Expression(name, type));
        }
        Args.Add((param.Name, arg));
      }

      // メンバ
      foreach (var childSyntax in node.Body.ChildNodes())
      {
        var memberResult = ItemFactory.Create(childSyntax, semanticModel, this);
        if (memberResult != null)
        {
          Members.Add(memberResult);
        }
      }

    }

    #region 基本インターフェース実装：メソッド

    /// <summary>
    /// 文字列取得
    /// </summary>
    /// <param name="index">前スペース数</param>
    /// <returns>文字列</returns>
    public override string ToString(int index = 0)
    {
      var result = new StringBuilder();
      var indexSpace = string.Concat(Enumerable.Repeat("  ", index));

      foreach (var comment in Comments)
      {
        result.Append(indexSpace);
        result.AppendLine($"{comment}");
      }

      foreach (var modifier in Modifiers)
      {
        result.Append(indexSpace);
        result.Append($"{modifier} ");
      }

      // メソッドの型
      MethodTypes.ForEach(type => result.Append(type.Name));
      
      // メソッド名
      result.Append($" {Name}");

      // パラメーター
      result.Append(" (");
      var isFirst = true;
      foreach (var arg in Args)
      {
        if (!isFirst)
        {
          result.Append(",");
        }
        arg.expressions.ForEach(item=> result.Append($"{item.Name}"));
        result.Append($" {arg.name}");
        isFirst = false;
      }
      result.AppendLine(") ");

      // メソッド内容
      result.Append(indexSpace);
      result.AppendLine("{");

      Members.ForEach(member => result.AppendLine(member.ToString(index + 1)));

      result.Append(indexSpace);
      result.AppendLine("}");

      return result.ToString();
    }

    #endregion

  }
}
