using CSharpAnalyze.Domain.Event;
using CSharpAnalyze.Domain.PublicInterfaces;
using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// アイテム：プロパティのアクセサクラス
  /// </summary>
  internal class ItemAccessor : AbstractItem, IItemAccessor
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="semanticModel">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    /// <param name="container">イベントコンテナ</param>
    public ItemAccessor(AccessorDeclarationSyntax node, SemanticModel semanticModel, IAnalyzeItem parent, EventContainer container) : base(parent, node, semanticModel, container)
    {
      ItemType = ItemTypes.Accessor;

      // キーワード
      Name = node.Keyword.ValueText;

      var statement = node.ChildNodes();
      if (!statement.Any())
      {
        return;
      }

      // メンバ
      var block = statement.First() as BlockSyntax;
      foreach (var childSyntax in block.Statements)
      {
        var memberResult = ItemFactory.Create(childSyntax, semanticModel, container, this);
        if (memberResult != null)
        {
          Members.Add(memberResult);
        }
      }
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="semanticModel">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    /// <param name="container">イベントコンテナ</param>
    public ItemAccessor(ArrowExpressionClauseSyntax node, SemanticModel semanticModel, IAnalyzeItem parent, EventContainer container) : base(parent, node, semanticModel, container)
    {
      ItemType = ItemTypes.Accessor;

      // メンバ
      var memberResult = ItemFactory.Create(node, semanticModel, container, this);
      Members.Add(memberResult);

      // キーワード
      if (memberResult is ItemReturn)
      {
        Name = "get";
      }
      else
      {
        Name = "set";
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

      // キーワード
      if (Members.Any())
      {
        result.AppendLine();
        result.AppendLine($"{indexSpace}{Name}");
        result.AppendLine(indexSpace + "{");

        foreach (var statement in Members)
        {
          result.AppendLine(statement.ToString(index + 1));
        }

        result.AppendLine(indexSpace + "}");
      }
      else
      {
        result.Append($"{Name};");
      }

      return result.ToString();
    }

    #endregion
  }
}
