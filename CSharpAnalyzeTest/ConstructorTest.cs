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
      StandardArgs,
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
        case CreatePattern.StandardArgs:
          filePath = "StandardArgs.cs";

          source.AppendLine("public class StandardArgs");
          source.AppendLine("{");
          source.AppendLine("  public StandardArgs(string str,int intger,float f,decimal d)");
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

        // クラス内の要素の存在確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions)>();

        Assert.Equal(expectedArgs.Count, GetMemberCount(itemClass, expectedModifiers, expectedArgs));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 値型パラメータのテスト
    /// </summary>
    [Fact(DisplayName = "StandardArgs")]
    public void StandardArgsTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.StandardArgs), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "StandardArgs.cs");

        // クラス内の要素の存在確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions)>()
        {
          ( "str","string"),
          ( "intger","int"),
          ( "f","float"),
          ( "d","decimal"),
        };

        Assert.Equal(expectedArgs.Count, GetMemberCount(itemClass, expectedModifiers, expectedArgs));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// メンバー数を取得
    /// </summary>
    /// <param name="itemClass">対象のアイテムクラス</param>
    /// <param name="modifiers">アクセス修飾子の期待値</param>
    /// <param name="expectedArgs">パラメータの期待値</param>
    /// <returns>条件が一致するメンバー数</returns>
    private int GetMemberCount(IItemClass itemClass, List<string> modifiers, List<(string name, string expressions)> expectedArgs)
    {
      // コンストラクタの存在確認
      var constructors = itemClass.Members.Where(member => member is IItemConstructor);
      Assert.Single(constructors);
      var constructor = constructors.First() as IItemConstructor;

      // アクセス修飾子の確認
      Assert.Equal(modifiers, constructor.Modifiers);

      // パラメータの確認
      var argCount = 0;
      foreach (var (name, expressions) in expectedArgs)
      {
        var actualArgs = constructor.Args
                        .Where(arg => arg.name == name)
                        .Where(arg => GetExpressions(arg.expressions) == expressions);
        if (actualArgs.Any())
        {
          argCount++;
        }
      }
      return argCount;
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
