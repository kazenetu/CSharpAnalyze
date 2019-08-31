using CSharpAnalyze.Domain.Event;
using CSharpAnalyze.Domain.PublicInterfaces;
using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// アイテム：ローカルメソッド
  /// </summary>
  internal class ItemLocalFunction : AbstractItem, IItemLocalFunction
  {
    /// <summary>
    /// メソッドの型リスト
    /// </summary>
    public List<IExpression> MethodTypes { get; } = new List<IExpression>();

    /// <summary>
    /// パラメーターリスト
    /// </summary>
    public List<(string name, List<IExpression> expressions, List<string> modifiers, List<IExpression> defaultValues)> Args { get; } = new List<(string name, List<IExpression> expressions, List<string> modifiers, List<IExpression> defaultValues)>();

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
    public ItemLocalFunction(LocalFunctionStatementSyntax node, SemanticModel semanticModel, IAnalyzeItem parent, EventContainer container) : base(parent, node, semanticModel, container)
    {
      ItemType = ItemTypes.MethodStatement;

      var declaredSymbol = (IMethodSymbol)semanticModel.GetDeclaredSymbol(node);

      // メソッドの型設定
      var parts = declaredSymbol.ReturnType.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat);
      foreach (var part in parts)
      {
        // スペースの場合は型設定に含めない
        if (part.Kind == SymbolDisplayPartKind.Space)
        {
          continue;
        }

        var name = Expression.GetSymbolName(part, true);
        var type = Expression.GetSymbolTypeName(part.Symbol);
        if (part.Kind == SymbolDisplayPartKind.ClassName)
        {
          // 外部ファイル参照イベント発行
          RaiseOtherFileReferenced(node, part.Symbol);
        }

        MethodTypes.Add(new Expression(name, type));
      }

      // パラメーター取得
      var nodeParams = node.ParameterList.Parameters;
      var paramIndex = 0;
      foreach (var param in declaredSymbol.Parameters)
      {
        var arg = new List<IExpression>();

        var argParts = param.Type.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat);
        foreach (var part in argParts)
        {
          var name = Expression.GetSymbolName(part, true);
          var type = Expression.GetSymbolTypeName(part.Symbol);
          if (part.Kind == SymbolDisplayPartKind.ClassName)
          {
            // 外部ファイル参照イベント発行
            RaiseOtherFileReferenced(node, part.Symbol);
          }

          arg.Add(new Expression(name, type));
        }

        // 参照型を設定
        var modifiers = new List<string>();
        switch (param.RefKind)
        {
          case RefKind.None:
            break;
          default:
            var modifiersArray = param.RefKind.ToString().ToLower(CultureInfo.CurrentCulture).Split(' ');
            modifiers.AddRange(modifiersArray);
            break;
        }

        // デフォルト値を設定
        var defaultValues = new List<IExpression>();
        if (param.HasExplicitDefaultValue)
        {
          var defaultValueOpration = semanticModel.GetOperation(nodeParams[paramIndex].Default) as IParameterInitializerOperation;
          defaultValues.AddRange(OperationFactory.GetExpressionList(defaultValueOpration.Value, container));
        }

        Args.Add((param.Name, arg, modifiers, defaultValues));

        paramIndex++;
      }

      // ジェネリックタイプ
      if (declaredSymbol.TypeParameters.Any())
      {
        var types = declaredSymbol.TypeParameters.Select(item => item.Name);
        GenericTypes.AddRange(types);
      }

      // メンバ
      if (node.Body is null)
      {
        var memberResult = ItemFactory.Create(node.ExpressionBody, semanticModel, container, this);
        if (memberResult != null)
        {
          Members.Add(memberResult);
        }
      }
      else
      {
        foreach (var childSyntax in node.Body.ChildNodes())
        {
          var memberResult = ItemFactory.Create(childSyntax, semanticModel, container, this);
          if (memberResult != null)
          {
            Members.Add(memberResult);
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

      // メソッドの型
      result.Append(indexSpace);
      MethodTypes.ForEach(type => result.Append(type.Name));

      // メソッド名
      result.Append($" {Name}");

      // パラメーター
      result.Append("(");
      var isFirst = true;
      foreach (var arg in Args)
      {
        if (!isFirst)
        {
          result.Append(",");
        }
        arg.modifiers.ForEach(item => result.Append($"{item} "));
        arg.expressions.ForEach(item => result.Append($"{item.Name}"));
        result.Append($" {arg.name}");

        if (arg.defaultValues.Any())
        {
          result.Append(" = ");
          arg.defaultValues.ForEach(item => result.Append($"{item.Name}"));
        }

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
