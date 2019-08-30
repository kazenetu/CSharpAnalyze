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
  /// アイテム：インターフェース
  /// </summary>
  internal class ItemInterface : AbstractItem, IItemInterface
  {
    /// <summary>
    /// インタフェースリスト
    /// </summary>
    public List<List<IExpression>> Interfaces { get; } = new List<List<IExpression>>();

    /// <summary>
    /// ジェネリックタイプリスト
    /// </summary>
    public List<string> GenericTypes { get; } = new List<string>();

    /// <summary>
    /// 継承元のプロパティリスト
    /// </summary>
    /// <remarks>参考情報</remarks>
    public List<string> BaseProperties { get; } = new List<string>();

    /// <summary>
    /// 継承元のメソッドリスト
    /// </summary>
    /// <remarks>参考情報</remarks>
    public List<string> BaseMethods { get; } = new List<string>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="semanticModel">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    /// <param name="container">イベントコンテナ</param>
    public ItemInterface(InterfaceDeclarationSyntax node, SemanticModel semanticModel, IAnalyzeItem parent, EventContainer container) : base(parent, node, semanticModel, container)
    {
      ItemType = ItemTypes.Interface;

      var declaredSymbol = semanticModel.GetDeclaredSymbol(node);

      // インターフェース設定
      if (node.BaseList != null)
      {
        // インターフェース
        foreach (var interfaceInfo in declaredSymbol.AllInterfaces)
        {
          Interfaces.Add(getExpressionList(interfaceInfo));

          // スーパーインタフェースのメンバーを追加する
          SetBaseMembers(interfaceInfo);
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

            var name = Expression.GetSymbolName(part, true);
            var type = Expression.GetSymbolTypeName(part.Symbol);
            if (part.Symbol != null)
            {
              type = part.Symbol.GetType().Name;
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
      if (declaredSymbol.TypeParameters.Any())
      {
        var types = declaredSymbol.TypeParameters.Select(item => item.Name);
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

    /// <summary>
    /// 継承元のメンバーを追加する
    /// </summary>
    /// <param name="target">インスタンス</param>
    private void SetBaseMembers(INamedTypeSymbol target)
    {
      // ソースファイル以外はそのまま終了
      if(!string.IsNullOrEmpty(target.ContainingAssembly.MetadataName)){
        return;
      }

      var targetName = target.Name;

      // 継承元からメンバを追加
      foreach (var interfaceInfo in target.AllInterfaces)
      {
        SetBaseMembers(interfaceInfo);
      }

      // メンバを追加
      foreach (var memberName in target.MemberNames)
      {
        var targetMembers = target.GetMembers(memberName);
        if (!targetMembers.Any())
        {
          continue;
        }

        foreach (var targetMember in targetMembers) 
        {
          var memberValue = targetMember.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
          memberValue = memberValue.Replace($"{targetName}.", string.Empty, StringComparison.CurrentCulture);
          switch (targetMember.Kind)
          {
            case SymbolKind.Property:
              var prop = targetMember as IPropertySymbol;
              var accessorList = new List<string>();
              if (prop.SetMethod != null){
                accessorList.Add("set;");
              }
              if (prop.GetMethod != null)
              {
                accessorList.Add("get;");
              }
              if(memberValue.Any())
              {
                memberValue += "{";
                foreach(var accessor in accessorList)
                {
                  memberValue += accessor;
                }
                memberValue += "}";
              }

              BaseProperties.Add(memberValue);
              break;
            case SymbolKind.Method:
              BaseMethods.Add(memberValue);
              break;
          }
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
      result.Append($"interface {Name}");

      // インターフェイス
      if (Interfaces.Any())
      {
        result.Append(" : ");

        Interfaces.ForEach(item => {
          if (Interfaces.IndexOf(item) > 0) result.Append(", ");
          item.ForEach(expression =>
          {
            result.Append(expression.Name);
          });
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
