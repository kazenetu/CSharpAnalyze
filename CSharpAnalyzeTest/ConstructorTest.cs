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
  [Trait("コンストラクタのテスト", nameof(FieldTest))]
  public class ConstructorTest : TestBase
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

          source.AppendLine("public class Standard");
          source.AppendLine("{");
          source.AppendLine("  public Standard()");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;
      }

      return new FileData(filePath, usingList.ToString(), source.ToString());
    }

    /// <summary>
    /// Setup
    /// </summary>
    public ConstructorTest() : base()
    {
      System.Diagnostics.Debug.WriteLine($"Setup {Environment.CurrentDirectory}");
    }

    /// <summary>
    /// コンストラクタのテスト
    /// </summary>
    [Fact(DisplayName = "Standard")]
    public void StandardTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Standard), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Standard.cs");

        // コンストラクタの存在確認
        var constructors = itemClass.Members.Where(member=>member is IItemConstructor);
        Assert.Single(constructors);
        var constructor = constructors.First() as IItemConstructor;

        // アクセス修飾子の確認
        var expectedModifiers = new List<string>() { "public" };
        Assert.Equal(expectedModifiers, constructor.Modifiers);

        // パラメータの確認
        var expectedArgs = new List<(string name, string expressions)>();

        var argCount = 0;
        foreach(var expectedArg in expectedArgs)
        {
          var actualArgs = constructor.Args
                          .Where(arg => arg.name == expectedArg.name)
                          .Where(arg => GetExpressions(arg.expressions) == expectedArg.expressions);
          if (actualArgs.Any())
          {
            argCount++;
          }
        }

        Assert.Equal(expectedArgs.Count, argCount);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// パラメーターリストを文字列に変換する
    /// </summary>
    /// <param name="expressions">パラメータリスト</param>
    /// <returns></returns>
    private string GetExpressions(List<IExpression> expressions)
    {
      return string.Join(",", expressions.Select(expression => expression.Name));
    }

  }
}
