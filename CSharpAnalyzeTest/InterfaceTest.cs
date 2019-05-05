using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyze.Domain.PublicInterfaces.Events;
using CSharpAnalyzeTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  [Trait("インターフェースのテスト", nameof(InterfaceTest))]
  public class InterfaceTest : TestBase
  {
    /// <summary>
    /// ソース作成パターン
    /// </summary>
    private enum CreatePattern
    {
      Standard,
    }

    /// <summary>
    /// ファイル名、ソースコード取得処理
    /// </summary>
    /// <param name="pattern">生成パターン</param>
    /// <returns>ファイルパスとソースコード</returns>
    private FileData CreateSource(CreatePattern pattern)
    {
      var usingList = new StringBuilder();
      var source = new StringBuilder();
      var filePath = string.Empty;

      switch (pattern)
      {
        case CreatePattern.Standard:
          filePath = "Standard.cs";

          source.AppendLine("public interface Inf ");
          source.AppendLine("{");
          source.AppendLine("}");
          break;
      }

      return new FileData(filePath, usingList.ToString(), source.ToString());
    }

    /// <summary>
    /// Setup
    /// </summary>
    public InterfaceTest() : base()
    {
      System.Diagnostics.Debug.WriteLine($"Setup {Environment.CurrentDirectory}");
    }

    /// <summary>
    /// 基本的なテスト
    /// </summary>
    [Fact(DisplayName = "Standard")]
    public void Standard()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Standard), (ev) =>
      {
        // ItemInterfaceインスタンスを取得
        var targets = GetIItemInterfaces(ev, "Standard.cs");

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象件数の確認
        Assert.Single(targets);

        // インターフェースの継承確認
        var target = targets.First();
        Assert.Empty(target.Interfaces);

        // インターフェースのメンバ確認
        var expectedMethodList = new List<(string methodName, string methodTypes, List<(string name, string expressions, string refType, string defaultValue)> expectedArgs)>();
        var expectedPropertyList = new List<(string name, string type, Dictionary<string, List<string>> accessors)>();

        var expectedCount = expectedMethodList.Count + expectedPropertyList.Count;
        var actualCount = GetMemberCount(target, expectedMethodList) + GetMemberCount(target, expectedPropertyList);

        Assert.Equal(expectedCount, actualCount);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// インターフェースインスタンスの取得
    /// </summary>
    /// <param name="ev">解析結果イベントインスタンス</param>
    /// <param name="filePath">ファイル名</param>
    /// <returns>インターフェースインスタンスリスト</returns>
    private List<IItemInterface> GetIItemInterfaces(IAnalyzed ev, string filePath)
    {
      // ファイル名の確認
      Assert.Equal(ev.FilePath, filePath);

      // 解析結果の存在確認
      Assert.NotNull(ev.FileRoot);

      // IItemInterfaceインスタンスの確認
      var targets = ev.FileRoot.Members.Where(item => item is IItemInterface);
      Assert.NotEmpty(targets);

      // IItemInterfaceインスタンスを返す
      return targets.Select(item => item as IItemInterface).ToList();
    }

    /// <summary>
    /// メンバー数を取得:メソッド用
    /// </summary>
    /// <param name="targetInstance">対象のインスタンス</param>
    /// <param name="expectedList">メソッド名とパラメータの期待値</param>
    /// <returns>条件が一致するメンバー数</returns>
    private int GetMemberCount(IItemInterface targetInstance, List<(string methodName, string methodTypes, List<(string name, string expressions, string refType, string defaultValue)> expectedArgs)> expectedList)
    {
      var memberCount = 0;
      foreach (var member in targetInstance.Members)
      {
        // 対象以外は次のmemberへ
        if (!(member is IItemMethod memberMethod)) continue;

        // 予想リストから同じ名前のメソッドを取得する
        var expectedMethods = expectedList.Where(item => item.methodName == member.Name);
        if (!expectedMethods.Any()) continue;

        // 予想リスト結果から対象が存在するか確認する
        foreach (var expectedMethod in expectedMethods)
        {
          // 型タイプの確認
          if (expectedMethod.methodTypes != GetExpressionsToString(memberMethod.MethodTypes))
            continue;

          // パラメータ取得
          var expectedArgs = expectedMethod.expectedArgs;

          // パラメータ数の確認
          if (expectedArgs.Count != memberMethod.Args.Count) continue;

          // パラメータの確認
          var argCount = 0;
          foreach (var (name, expressions, refType, defaultValue) in expectedArgs)
          {
            var actualArgs = memberMethod.Args
                            .Where(arg => arg.name == name)
                            .Where(arg => GetExpressionsToString(arg.expressions) == expressions)
                            .Where(arg => string.Concat(arg.modifiers) == refType)
                            .Where(arg => GetExpressionsToString(arg.defaultValues) == defaultValue);
            if (actualArgs.Any())
            {
              argCount++;
            }
          }

          // 一致パラメータ数の確認
          Assert.Equal(expectedArgs.Count, argCount);
        }

        memberCount++;
      }

      return memberCount;
    }

    /// <summary>
    /// メンバー数を取得：プロパティ
    /// </summary>
    /// <param name="targetInstance">対象のインスタンス</param>
    /// <param name="expectedList">予想値リスト</param>
    /// <returns>条件が一致するメンバー数</returns>
    private int GetMemberCount(IItemInterface targetInstance, List<(string name, string type, Dictionary<string, List<string>> accessors)> expectedList)
    {
      var memberCount = 0;
      foreach (var member in targetInstance.Members)
      {
        // 対象以外は次のmemberへ
        if (!(member is IItemProperty memberProperty)) continue;

        // 型の一致確認
        var targetProperties = expectedList.Where(field => field.name == memberProperty.Name && field.type == GetExpressionsToString(memberProperty.PropertyTypes));
        if (!targetProperties.Any()) continue;

        // 条件取得
        var (name, type, accessors) = targetProperties.First();

        // アクセサの一致確認
        var accessorCount = 0;
        foreach (var expectedItem in accessors)
        {
          // 対象のアクセサの存在確認
          var targets = memberProperty.AccessorList.Where(accessor => accessor.Name == expectedItem.Key);
          Assert.Single(targets);

          // 対象を取得
          var target = targets.First();

          // コード数の一致確認
          Assert.Equal(expectedItem.Value.Count, target.Members.Count);

          // コードの確認
          var accessorMemberCount = 0;
          foreach (var expectedMember in expectedItem.Value)
          {
            Assert.Equal(expectedMember, target.Members[accessorMemberCount].ToString());
            accessorMemberCount++;
          }

          accessorCount++;
        }
        // 確認済アクセサ数の一致確認
        Assert.Equal(accessorCount, memberProperty.AccessorList.Count);

        memberCount++;
      }

      return memberCount;
    }
  }
}
