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
      ClassArgs,
      ListArgs,
      CallSuperConstructor,
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

        case CreatePattern.ClassArgs:
          filePath = "ClassArgs.cs";

          source.AppendLine("public class ClassArgs");
          source.AppendLine("{");
          source.AppendLine("  public ClassArgs(Standard instance)");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.ListArgs:
          filePath = "ListArgs.cs";

          source.AppendLine("public class ListArgs");
          source.AppendLine("{");
          source.AppendLine("  public ListArgs(List<string> list)");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.CallSuperConstructor:
          filePath = "CallSuperConstructor.cs";

          source.AppendLine("public class CallSuperConstructor : StandardArgs");
          source.AppendLine("{");
          source.AppendLine("  public CallSuperConstructor(string str1,int integer1,float f1,decimal d1,int integer2):base(str1,integer1,f1,d1)");
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

        // IItemConstructorsインスタンスのリストを取得
        var constructors = GetIItemConstructors(itemClass);

        // constructorインスタンスを取得
        Assert.Single(constructors);
        var constructor = constructors.First() as IItemConstructor;

        // クラス内の要素の存在確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions)>();
        Assert.Equal(expectedArgs.Count, GetMemberCount(constructor, expectedModifiers, expectedArgs));

        // スーパークラスのコンストラクタ呼び出し確認
        var expectedBaseArgs = new List<string>();
        Assert.Equal(expectedBaseArgs, constructor.BaseArgs);
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

        // IItemConstructorsインスタンスのリストを取得
        var constructors = GetIItemConstructors(itemClass);

        // constructorインスタンスを取得
        Assert.Single(constructors);
        var constructor = constructors.First() as IItemConstructor;

        // クラス内の要素の存在確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions)>()
        {
          ( "str","string"),
          ( "intger","int"),
          ( "f","float"),
          ( "d","decimal"),
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(constructor, expectedModifiers, expectedArgs));

        // スーパークラスのコンストラクタ呼び出し確認
        var expectedBaseArgs = new List<string>();
        Assert.Equal(expectedBaseArgs, constructor.BaseArgs);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// クラスインスタンスのパラメータのテスト
    /// </summary>
    [Fact(DisplayName = "ClassArgs")]
    public void ClassArgsTest()
    {
      CreateFileData(CreateSource(CreatePattern.Standard), null);

      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ClassArgs), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ClassArgs.cs");

        // IItemConstructorsインスタンスのリストを取得
        var constructors = GetIItemConstructors(itemClass);

        // constructorインスタンスを取得
        Assert.Single(constructors);
        var constructor = constructors.First() as IItemConstructor;

        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);
        Assert.Equal("Standard", ev.FileRoot.OtherFiles.First().Key);
        Assert.Equal("Standard.cs", ev.FileRoot.OtherFiles.First().Value);

        // クラス内の要素の存在確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions)>()
        {
          ( "instance","Standard"),
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(constructor, expectedModifiers, expectedArgs));

        // スーパークラスのコンストラクタ呼び出し確認
        var expectedBaseArgs = new List<string>();
        Assert.Equal(expectedBaseArgs, constructor.BaseArgs);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 組み込みジェネリックインスタンスのパラメータのテスト
    /// </summary>
    [Fact(DisplayName = "ListArgs")]
    public void ListArgsTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ListArgs), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ListArgs.cs");

        // IItemConstructorsインスタンスのリストを取得
        var constructors = GetIItemConstructors(itemClass);

        // constructorインスタンスを取得
        Assert.Single(constructors);
        var constructor = constructors.First() as IItemConstructor;

        // クラス内の要素の存在確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions)>()
        {
          ( "list","List<string>"),
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(constructor, expectedModifiers, expectedArgs));

        // スーパークラスのコンストラクタ呼び出し確認
        var expectedBaseArgs = new List<string>();
        Assert.Equal(expectedBaseArgs, constructor.BaseArgs);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// スーパークラスのコンストラクタ呼び出しのテスト
    /// </summary>
    [Fact(DisplayName = "CallSuperConstructor")]
    public void BaseArgsTest()
    {
      CreateFileData(CreateSource(CreatePattern.StandardArgs), null);

      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.CallSuperConstructor), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "CallSuperConstructor.cs");

        // IItemConstructorsインスタンスのリストを取得
        var constructors = GetIItemConstructors(itemClass);

        // constructorインスタンスを取得
        Assert.Single(constructors);
        var constructor = constructors.First() as IItemConstructor;

        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);
        Assert.Equal("StandardArgs", ev.FileRoot.OtherFiles.First().Key);
        Assert.Equal("StandardArgs.cs", ev.FileRoot.OtherFiles.First().Value);

        // クラス内の要素の存在確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions)>()
        {
          ( "str1","string"),
          ( "integer1","int"),
          ( "f1","float"),
          ( "d1","decimal"),
          ( "integer2","int"),
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(constructor, expectedModifiers, expectedArgs));

        // スーパークラスのコンストラクタ呼び出し確認
        var expectedBaseArgs = new List<string>()
        {
          "str1",
          "integer1",
          "f1",
          "d1",
        };
        Assert.Equal(expectedBaseArgs, constructor.BaseArgs);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// コンストラクタインスタンスの取得
    /// </summary>
    /// <param name="itemClass">対象のアイテムクラス</param>
    /// <returns>コンストラクタインスタンスリスト</returns>
    private List<IItemConstructor> GetIItemConstructors(IItemClass itemClass)
    {
      return itemClass.Members.Where(member => member is IItemConstructor).
              Select(constructor=> constructor as IItemConstructor).ToList();
    }

    /// <summary>
    /// メンバー数を取得
    /// </summary>
    /// <param name="itemConstructor">対象のコンストラクタクラス</param>
    /// <param name="modifiers">アクセス修飾子の期待値</param>
    /// <param name="expectedArgs">パラメータの期待値</param>
    /// <returns>条件が一致するメンバー数</returns>
    private int GetMemberCount(IItemConstructor itemConstructor, List<string> modifiers, List<(string name, string expressions)> expectedArgs)
    {
      // アクセス修飾子の確認
      Assert.Equal(modifiers, itemConstructor.Modifiers);

      // パラメータの確認
      var argCount = 0;
      foreach (var (name, expressions) in expectedArgs)
      {
        var actualArgs = itemConstructor.Args
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
      return string.Concat(expressions.Select(expression => expression.Name));
    }

  }
}
