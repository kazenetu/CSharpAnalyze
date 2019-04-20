using CSharpAnalyze.Domain.Event;
using CSharpAnalyze.Domain.PublicInterfaces;
using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
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
  /// アイテム：クラス
  /// </summary>
  internal class ItemClass : AbstractItem, IItemClass
  {
    /// <summary>
    /// スーパークラスリスト
    /// </summary>
    public List<IExpression> SuperClass { get; } = new List<IExpression>();

    /// <summary>
    /// インタフェースリスト
    /// </summary>
    public List<List<IExpression>> Interfaces { get; } = new List<List<IExpression>>();

    /// <summary>
    /// ジェネリックタイプリスト
    /// </summary>
    public List<string> GenericTypes { get; } = new List<string>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="semanticModel">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    /// <param name="container">イベントコンテナ</param>
    public ItemClass(ClassDeclarationSyntax node, SemanticModel semanticModel, IAnalyzeItem parent, EventContainer container) : base(parent, node, semanticModel, container)
    {
      ItemType = ItemTypes.Class;

      var declaredClass = semanticModel.GetDeclaredSymbol(node);

      // スーパークラス/インターフェース設定
      if (node.BaseList != null)
      {
        // スーパークラス
        SuperClass.AddRange(getExpressionList(declaredClass.BaseType));

        // インターフェース
        foreach(var interfaceInfo in declaredClass.AllInterfaces)
        {
          Interfaces.Add(getExpressionList(interfaceInfo));
        }

        // 対象をList<IExpression>に格納する
        List<IExpression> getExpressionList(INamedTypeSymbol target)
        {
          var result = new List<IExpression>();

          var displayParts = target.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat);
          foreach (var part in displayParts)
          {
            // スペースの場合は型設定に含めない
            if (part.Kind == SymbolDisplayPartKind.Space)
            {
              continue;
            }

            var name = $"{part}";
            var type = Expression.GetSymbolTypeName(part.Symbol);
            if (part.Symbol != null)
            {
              type = part.Symbol.GetType().Name;
              if (!string.IsNullOrEmpty(part.Symbol.ContainingNamespace.Name))
              {
                name = $"{part.Symbol}".Replace($"{part.Symbol.ContainingNamespace}.", string.Empty, StringComparison.CurrentCulture);
              }

              if (part.Kind == SymbolDisplayPartKind.ClassName || part.Kind == SymbolDisplayPartKind.InterfaceName)
              {
                // 外部ファイル参照イベント発行
                RaiseOtherFileReferenced(node, part.Symbol);
              }
            }

            result.Add(new Expression(name, type));
          }

          return result;
        }
      }

      // ジェネリックタイプ
      if (declaredClass.TypeParameters.Any())
      {
        var types = declaredClass.TypeParameters.Select(item => item.Name);
        GenericTypes.AddRange(types);
      }

      // メンバ
      foreach (var childSyntax in node.ChildNodes())
      {
        var memberResult = ItemFactory.Create(childSyntax, semanticModel, container, this);
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
      result.Append($"class {Name}");

      // ジェネリックタイプ
      if (GenericTypes.Any())
      {
        result.Append("<");

        GenericTypes.ForEach(item => {
          result.Append(item);
          if (GenericTypes.IndexOf(item) > 0) result.Append(", ");
        });

        result.Append(">");
      }

      // スーパークラス/インターフェイス
      if (SuperClass.Any())
      {
        result.Append(" : ");
        SuperClass.ForEach(item => {
          result.Append(item.Name);
          if (SuperClass.IndexOf(item) > 0) result.Append(", ");
        });
      }
      result.AppendLine();
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
