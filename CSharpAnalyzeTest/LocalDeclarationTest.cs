using CSharpAnalyze.Domain.PublicInterfaces;
using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyzeTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  [Trait("ローカル変数のテスト", nameof(LocalDeclarationTest))]
  public class LocalDeclarationTest : TestBase
  {
    /// <summary>
    /// ソース作成パターン
    /// </summary>
    private enum CreatePattern
    {
      Standard,
      TypeInference,
    }

    /// <summary>
    /// ファイル名、ソースコード取得処理
    /// </summary>
    /// <param name="pattern">生成パターン</param>
    /// <returns>ファイルパスとソースコード</returns>
    private FileData CreateSource(CreatePattern pattern)
    {
      var usingList = new StringBuilder();
      var source = new List<string>();
      var filePath = string.Empty;

      switch (pattern)
      {
        case CreatePattern.Standard:
          filePath = "Standard.cs";

          source.Add("string target;");
          source.Add("int targetInt = 1;");
          break;

        case CreatePattern.TypeInference:
          filePath = "TypeInference.cs";

          source.Add("var target = \"ABC\";");
          source.Add("var targetInt = 1;");
          break;
      }

      // ソースコード作成
      var targetSource = new StringBuilder();
      targetSource.AppendLine("public class Standard");
      targetSource.AppendLine("{");
      targetSource.AppendLine("  public void TestMethod()");
      targetSource.AppendLine("  {");

      source.ForEach(line => targetSource.AppendLine($"    {line }"));

      targetSource.AppendLine("  }");
      targetSource.AppendLine("}");

      return new FileData(filePath, usingList.ToString(), targetSource.ToString());
    }

    /// <summary>
    /// Setup
    /// </summary>
    public LocalDeclarationTest() : base()
    {
      System.Diagnostics.Debug.WriteLine($"Setup {Environment.CurrentDirectory}");
    }

    /// <summary>
    /// 基本的なテスト
    /// </summary>
    [Fact(DisplayName = "Standard")]
    public void StandardArgsTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Standard), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Standard.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // パラメータの確認
        var expectedArgs = new List<(string type, string name, string defaultValue)>()
        {
          ("string", "target", null),
          ("int", "targetInt", "1")
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetParentInstance, expectedArgs));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 型推論のテスト
    /// </summary>
    [Fact(DisplayName = "TypeInference")]
    public void TypeInferenceTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.TypeInference), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "TypeInference.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // パラメータの確認
        var expectedArgs = new List<(string type, string name, string defaultValue)>()
        {
          ("string", "target", "\"ABC\""),
          ("int", "targetInt", "1")
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetParentInstance, expectedArgs));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 対象インスタンスの取得
    /// </summary>
    /// <param name="itemClass">対象のアイテムクラス</param>
    /// <returns>対象インスタンスリスト</returns>
    private List<IItemMethod> GetTargetInstances(IItemClass itemClass)
    {
      return itemClass.Members.Where(member => member is IItemMethod).
              Select(member => member as IItemMethod).ToList();
    }

    /// <summary>
    /// メンバー数を取得
    /// </summary>
    /// <param name="targetParentInstance">対象の親インスタンス</param>
    /// <param name="expectedList">予想値リスト</param>
    /// <returns>条件が一致するメンバー数</returns>
    private int GetMemberCount(IItemMethod targetParentInstance, List<(string type, string name, string defaultValue)> expectedList)
    {
      var memberCount = 0;
      foreach (var member in targetParentInstance.Members)
      {
        // 対象以外は次のmemberへ
        if (!(member is IItemStatementLocalDeclaration targetMember)) continue;

        // 型の一致確認
        var expectedTargets = expectedList.Where(expected => expected.name == targetMember.Name && expected.type == GetExpressions(targetMember.Types));
        if (!expectedTargets.Any()) continue;

        // 予想値を取得
        var expectedTarget = expectedTargets.First();

        // 初期値の確認
        if (expectedTarget.defaultValue is null)
        {
          Assert.Empty(targetMember.DefaultValues);
        }
        else
        {
          Assert.Equal(expectedTarget.defaultValue, GetExpressions(targetMember.DefaultValues));
        }

        memberCount++;
      }
      return memberCount;
    }

    /// <summary>
    /// パラメーターリストを文字列に変換する
    /// </summary>
    /// <param name="expressions">パラメータリスト</param>
    /// <returns></returns>
    private string GetExpressions(List<IExpression> expressions)
    {
      return string.Concat(expressions.Select(expression => expression.Name));
    }
  }
}
