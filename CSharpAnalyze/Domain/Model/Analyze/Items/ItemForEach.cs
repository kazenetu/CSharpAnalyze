using CSharpAnalyze.Domain.Event;
using CSharpAnalyze.Domain.PublicInterfaces;
using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// アイテム：foreach
  /// </summary>
  internal class ItemForEach : AbstractItem, IItemForEach
  {
    /// <summary>
    /// ローカル
    /// </summary>
    public List<IExpression> Local { get; } = new List<IExpression>();

    /// <summary>
    /// ローカルが型推論か否か
    /// </summary>
    public bool IsVar { get; }

    /// <summary>
    /// ローカルの型リスト
    /// </summary>
    public List<IExpression> LocalTypes { get; } = new List<IExpression>();

    /// <summary>
    /// コレクション
    /// </summary>
    public List<IExpression> Collection { get; } = new List<IExpression>();

    /// <summary>
    /// コレクションの型リスト
    /// </summary>
    public List<IExpression> CollectionTypes { get; } = new List<IExpression>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="semanticModel">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    /// <param name="container">イベントコンテナ</param>
    public ItemForEach(ForEachStatementSyntax node, SemanticModel semanticModel, IAnalyzeItem parent, EventContainer container) : base(parent, node, semanticModel, container)
    {
      ItemType = ItemTypes.MethodStatement;

      var oparetion = semanticModel.GetOperation(node) as IForEachLoopOperation;
      var localSymbol = oparetion.Locals.First();

      // ローカルの型設定
      IsVar = node.Type.IsVar;
      LocalTypes.AddRange(GetTypes(localSymbol.Type, semanticModel, node));

      // ローカル設定
      Local.Add(new Expression(localSymbol.Name, Expression.GetSymbolTypeName(localSymbol)));

      // コレクションの型設定
      var conversionOperation = oparetion.Collection as IConversionOperation;
      if(!(conversionOperation is null)){
        CollectionTypes.AddRange(GetTypes(conversionOperation.Operand.Type, semanticModel, node));
      }

      //コレクション
      Collection.AddRange(OperationFactory.GetExpressionList(oparetion.Collection.Children.First(), container));

      // 内部処理設定
      var block = node.Statement as BlockSyntax;
      foreach (var statement in block.Statements)
      {
        Members.Add(ItemFactory.Create(statement, semanticModel, container, this));
      }
    }

    /// <summary>
    /// 型リストを取得する
    /// </summary>
    /// <param name="symbol">対象インスタンス</param>
    /// <param name="semanticModel">対象ソースのsemanticModel</param>
    /// <param name="node">対象Node</param>
    /// <returns>型リスト</returns>
    private List<IExpression> GetTypes(ISymbol symbol, SemanticModel semanticModel, SyntaxNode node, SymbolDisplayFormat format = null)
    {
      var result = new List<IExpression>();

      var symbolDisplayFormat = format;
      if(symbolDisplayFormat is null){
        symbolDisplayFormat = SymbolDisplayFormat.MinimallyQualifiedFormat;
        symbolDisplayFormat = SymbolDisplayFormat.CSharpErrorMessageFormat;
      }

      var existsNamespace = false;
      var parts = symbol.ToDisplayParts(symbolDisplayFormat);
      foreach (var part in parts)
      {
        // スペースの場合は型設定に含めない
        if (part.Kind == SymbolDisplayPartKind.Space)
        {
          continue;
        }

        // Namespaceの場合は型設定に含めない
        if (part.Kind == SymbolDisplayPartKind.NamespaceName)
        {
          existsNamespace = true;
          continue;
        }

        // ピリオドが初回の場合はキャンセル
        if(existsNamespace && $"{part}" == ".")
        {
          existsNamespace = false;
          continue;
        }

        var name = Expression.GetSymbolName(part, true);
        var type = Expression.GetSymbolTypeName(part.Symbol);
        if (part.Kind == SymbolDisplayPartKind.ClassName)
        {
          // 外部ファイル参照イベント発行
          RaiseOtherFileReferenced(node, part.Symbol);

          // 親がクラスの場合は内部クラス
          if(part.Symbol.ContainingSymbol is INamedTypeSymbol parentSymbol && parentSymbol.TypeKind==TypeKind.Class)
          {
            // 内部クラス名を設定
            name = $"{part}";
          }
        }

        result.Add(new Expression(name, type));
      }

      return result;
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

      result.Append(indexSpace);
      result.Append("foreach(");
      result.Append(Local.First().Name);
      result.Append(" in ");
      result.Append(Collection.First().Name);
      result.AppendLine(")");

      result.Append(indexSpace);
      result.AppendLine("{");
      foreach (var statement in Members)
      {
        result.AppendLine(statement.ToString(index + 1));
      }
      result.Append(indexSpace);
      result.AppendLine("}");

      return result.ToString();
    }

    #endregion
  }
}
