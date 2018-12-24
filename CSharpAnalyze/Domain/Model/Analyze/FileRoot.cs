using CSharpAnalyze.Domain.Event;
using CSharpAnalyze.Domain.Event.Analyze;
using CSharpAnalyze.Domain.PublicInterfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze
{
  /// <summary>
  /// ファイルルート
  /// </summary>
  internal class FileRoot : IFileRoot
  {
    /// <summary>
    /// ファイルパス
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// 外部参照のクラス名とファイルパスのリスト
    /// </summary>
    public Dictionary<string, string> OtherFiles { get; } = new Dictionary<string, string>();

    /// <summary>
    /// 子メンバ
    /// </summary>
    public List<IAnalyzeItem> Members { get; } = new List<IAnalyzeItem>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="target">セマンティックモデル</param>
    public FileRoot(SemanticModel target)
    {
      // ファイルパス取得
      FilePath = target.SyntaxTree.FilePath;

      // 外部参照イベント登録
      EventContainer.Register<OtherFileReferenced>(this, (ev) =>
      {
        if (!OtherFiles.ContainsKey(ev.ClassName))
        {
          OtherFiles.Add(ev.ClassName, ev.FilePath);
        }
      });

      // 解析処理
      var rootNode = target.SyntaxTree.GetRoot().ChildNodes().Where(syntax => syntax.IsKind(SyntaxKind.NamespaceDeclaration)).First();
      foreach (var item in (rootNode as NamespaceDeclarationSyntax).Members)
      {
        var memberResult = ItemFactory.Create(item, target);
        if (memberResult != null)
        {
          Members.Add(memberResult);
        }
      }

      // 外部参照イベント登録解除
      EventContainer.Unregister<OtherFileReferenced>(this);
    }

    /// <summary>
    /// 文字列取得
    /// </summary>
    /// <returns>文字列</returns>
    public override string ToString()
    {
      var result = new StringBuilder();

      // 外部参照ファイル
      foreach (var otherFile in OtherFiles)
      {
        result.AppendLine($"OtherFileReference：[{otherFile.Key}] in [{otherFile.Value}]");
      }

      // メンバー
      Members.ForEach(member => result.AppendLine(member.ToString(0)));
      return result.ToString();
    }

    /// <summary>
    /// FileRootインスタンス作成
    /// </summary>
    /// <param name="target">セマンティックモデル</param>
    /// <returns>FileRootインスタンス</returns>
    /// <remarks>Analyzedイベントで結果を返す</remarks>
    public static IFileRoot Create(SemanticModel target)
    {
      // ファイル情報取得
      var instance = new FileRoot(target);

      // イベント発行：解析完了
      EventContainer.Raise(new Analyzed(instance));

      // ファイル情報を返す
      return instance;
    }
  }
}
