using CSharpAnalyze.Domain.Event;
using CSharpAnalyze.Domain.Event.Analyze;
using CSharpAnalyze.Domain.Model.Analyze.Items;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace CSharpAnalyze.Domain.Model.Analyze
{
  /// <summary>
  /// セマンティックモデル解析クラス(実装中)
  /// </summary>
  /// <remarks>廃止予定</remarks>
  public class SemanticModelAnalyze
  {
    public SemanticModelAnalyze(SemanticModel target)
    {
      var analyzeResult = new ItemRoot();

      // 外部参照イベント登録
      EventContainer.Register<OtherFileReferenced>(this, (ev) =>
      {
        if (!analyzeResult.OtherFiles.ContainsKey(ev.ClassName))
        {
          analyzeResult.OtherFiles.Add(ev.ClassName, ev.FilePath);
        }
      });

      // 解析処理
      var rootNode = target.SyntaxTree.GetRoot().ChildNodes().Where(syntax => syntax.IsKind(SyntaxKind.NamespaceDeclaration)).First();
      foreach (var item in (rootNode as NamespaceDeclarationSyntax).Members)
      {
        var memberResult = ItemFactory.Create(item, target);
        if (memberResult != null)
        {
          analyzeResult.Members.Add(memberResult);
        }
      }

      // 外部参照イベント登録解除
      EventContainer.Unregister<OtherFileReferenced>(this);

      // イベント発行：解析完了
      EventContainer.Raise(new Analyzed($"[{rootNode.SyntaxTree.FilePath}]", analyzeResult));
    }
  }
}
