using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyzeTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  [Trait("式のテスト", nameof(ExpressionTest))]
  public class ExpressionTest : TestBase
  {
    /// <summary>
    /// ソース作成パターン
    /// </summary>
    private enum CreatePattern
    {
      SimpleAssignment,
      Invocation,
      PropertyReference,
      LocalReference,
      Literal,
      FieldReference,
      InstanceReference,
      CompoundAssignment,
      IncrementOrDecrement,
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
        case CreatePattern.SimpleAssignment:
          filePath = "SimpleAssignment.cs";

          source.Add("int a;");
          source.Add("a=1;");
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
    public ExpressionTest() : base()
    {
      System.Diagnostics.Debug.WriteLine($"Setup {Environment.CurrentDirectory}");
    }

    /// <summary>
    /// 代入式のテスト
    /// </summary>
    [Fact(DisplayName = "SimpleAssignment")]
    public void SimpleAssignmentTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.SimpleAssignment), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "SimpleAssignment.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);
        Assert.Single(targetInstances);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // パラメータの確認
        var expectedArgs = new List<(string left, string operatorToken, string right)>()
        {
          ("a", "=", "1"),
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetParentInstance, expectedArgs));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    #region ユーティリティメソッド
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
    private int GetMemberCount(IItemMethod targetParentInstance, List<(string left, string operatorToken, string right)> expectedList)
    {
      var memberCount = 0;
      foreach (var member in targetParentInstance.Members)
      {
        // 対象以外は次のmemberへ
        if (!(member is IItemStatementExpression targetMember)) continue;

        // 対象取得
        var expectedTargets = 
            expectedList.Where(expected => expected.left == GetExpressionsToString(targetMember.LeftSideList))
                        .Where(expected => expected.operatorToken == targetMember.AssignmentOperator)
                        .Where(expected => expected.right == GetExpressionsToString(targetMember.RightSideList));
        if (!expectedTargets.Any()) continue;

        memberCount++;
      }
      return memberCount;
    }
    #endregion

  }
}
