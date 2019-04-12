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
  [Trait("コンストラクタのテスト", nameof(ConstructorTest))]
  [Collection("CollectionName")]
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
      Multiple,
      RefArgs,
      DefaultValues,
      Lambda,
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

        case CreatePattern.Multiple:
          filePath = "Multiple.cs";

          source.AppendLine("public class Multiple");
          source.AppendLine("{");
          source.AppendLine("  public Multiple(string str)");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("  private Multiple(string str1,int integer)");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.RefArgs:
          filePath = "RefArgs.cs";

          source.AppendLine("public class RefArgs");
          source.AppendLine("{");
          source.AppendLine("  public RefArgs(ref int integer,in string str,out decimal output)");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.DefaultValues:
          filePath = "DefaultValues.cs";

          source.AppendLine("public class DefaultValues");
          source.AppendLine("{");
          source.AppendLine("  public DefaultValues(int integer=10,string str=\"ABC\")");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.Lambda:
          filePath = "Lambda.cs";

          source.AppendLine("public class Lambda");
          source.AppendLine("{");
          source.AppendLine("  private int field;");
          source.AppendLine("  public Lambda() => field = 10;");
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

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>();
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

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // クラス内の要素の存在確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
          ( "str","string","",""),
          ( "intger","int","",""),
          ( "f","float","",""),
          ( "d","decimal","",""),
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(constructor, expectedModifiers, expectedArgs));

        // パラメータの確認
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

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
          ( "instance","Standard","",""),
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

        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);
        Assert.Equal("List", ev.FileRoot.OtherFiles.First().Key);
        Assert.Equal("", ev.FileRoot.OtherFiles.First().Value);

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
          ( "list","List<string>","",""),
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

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
          ( "str1","string","",""),
          ( "integer1","int","",""),
          ( "f1","float","",""),
          ( "d1","decimal","",""),
          ( "integer2","int","",""),
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
    /// 複数コンストラクタ呼び出しのテスト
    /// </summary>
    [Fact(DisplayName = "Multiple")]
    public void MultipleTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Multiple), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Multiple.cs");

        // IItemConstructorsインスタンスのリストを取得
        var constructors = GetIItemConstructors(itemClass);

        // constructorインスタンスを取得
        Assert.True(constructors.Count == 2, $"constructors != 2 [{constructors.Count}]");

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // クラス内の要素の想定値を設定
        var expectedModifiersList = new List<List<string>>() {
          new List<string>{ "public" },
          new List<string>{ "private" },
        };
        var expectedArgsList = new List<List<(string name, string expressions, string refType, string defaultValue)>>()
        {
          new List<(string name, string expressions, string refType, string defaultValue)>{
            ( "str","string","",""),
          },
          new List<(string name, string expressions, string refType, string defaultValue)>{
            ( "str1","string","",""),
            ( "integer","int","",""),
          },
        };

        var expectedIndex = 0;
        foreach (IItemConstructor constructor in constructors)
        {
          // パラメータの確認
          var expectedModifiers = expectedModifiersList[expectedIndex];
          var expectedArgs = expectedArgsList[expectedIndex];
          Assert.Equal(expectedArgs.Count, GetMemberCount(constructor, expectedModifiers, expectedArgs));

          // スーパークラスのコンストラクタ呼び出し確認
          Assert.Equal(new List<string>(), constructor.BaseArgs);

          expectedIndex++;
        }
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 参照系のパラメータのテスト
    /// </summary>
    [Fact(DisplayName = "RefArgs")]
    public void RefArgsTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.RefArgs), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "RefArgs.cs");

        // IItemConstructorsインスタンスのリストを取得
        var constructors = GetIItemConstructors(itemClass);

        // constructorインスタンスを取得
        Assert.Single(constructors);
        var constructor = constructors.First() as IItemConstructor;

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
          ( "integer","int","ref",""),
          ( "str","string","in",""),
          ( "output","decimal","out",""),
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
    /// 初期値パラメータのテスト
    /// </summary>
    [Fact(DisplayName = "DefaultValues")]
    public void DefaultValuesTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.DefaultValues), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "DefaultValues.cs");

        // IItemConstructorsインスタンスのリストを取得
        var constructors = GetIItemConstructors(itemClass);

        // constructorインスタンスを取得
        Assert.Single(constructors);
        var constructor = constructors.First() as IItemConstructor;

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
          ( "integer","int","","10"),
          ( "str","string","","\"ABC\""),
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
    /// 式形式のテスト
    /// </summary>
    [Fact(DisplayName = "Lambda")]
    public void LambdaTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Lambda), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Lambda.cs");

        // IItemConstructorsインスタンスのリストを取得
        var constructors = GetIItemConstructors(itemClass);

        // constructorインスタンスを取得
        Assert.Single(constructors);
        var constructor = constructors.First() as IItemConstructor;

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
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
    private int GetMemberCount(IItemConstructor itemConstructor, List<string> modifiers, List<(string name, string expressions, string refType, string defaultValue)> expectedArgs)
    {
      // アクセス修飾子の確認
      Assert.Equal(modifiers, itemConstructor.Modifiers);

      // パラメータ数の確認
      Assert.Equal(expectedArgs.Count, itemConstructor.Args.Count);

      // パラメータの確認
      var argCount = 0;
      foreach (var (name, expressions, refType, defaultValue) in expectedArgs)
      {
        var actualArgs = itemConstructor.Args
                        .Where(arg => arg.name == name)
                        .Where(arg => GetExpressions(arg.expressions) == expressions)
                        .Where(arg => string.Concat(arg.modifiers) == refType)
                        .Where(arg => GetExpressions(arg.defaultValues) == defaultValue);
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
