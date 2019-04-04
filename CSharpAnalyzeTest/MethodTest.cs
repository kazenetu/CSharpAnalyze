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
  [Trait("メソッドのテスト", nameof(MethodTest))]
  public class MethodTest : TestBase
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
      Multiple,
      RefArgs,
      DefaultValues,
      ReturnValue,
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
          source.AppendLine("  public void TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.StandardArgs:
          filePath = "StandardArgs.cs";

          source.AppendLine("public class StandardArgs");
          source.AppendLine("{");
          source.AppendLine("  public void TestMethod(string str,int intger,float f,decimal d)");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.ClassArgs:
          filePath = "ClassArgs.cs";

          source.AppendLine("public class ClassArgs");
          source.AppendLine("{");
          source.AppendLine("  public void TestMethod(Standard instance)");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.ListArgs:
          filePath = "ListArgs.cs";

          source.AppendLine("public class ListArgs");
          source.AppendLine("{");
          source.AppendLine("  public void TestMethod(List<string> list)");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.Multiple:
          filePath = "Multiple.cs";

          source.AppendLine("public class Multiple");
          source.AppendLine("{");
          source.AppendLine("  public void TestMethod(string str)");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("  private void TestMethod(string str1,int integer)");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.RefArgs:
          filePath = "RefArgs.cs";

          source.AppendLine("public class RefArgs");
          source.AppendLine("{");
          source.AppendLine("  public void TestMethod(ref int integer,in string str,out decimal output)");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.DefaultValues:
          filePath = "DefaultValues.cs";

          source.AppendLine("public class DefaultValues");
          source.AppendLine("{");
          source.AppendLine("  public void TestMethod(int integer=10,string str=\"ABC\")");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.ReturnValue:
          filePath = "ReturnValue.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public int TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    return 10;");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;
      }

      return new FileData(filePath, usingList.ToString(), source.ToString());
    }

    /// <summary>
    /// Setup
    /// </summary>
    public MethodTest() : base()
    {
      System.Diagnostics.Debug.WriteLine($"Setup {Environment.CurrentDirectory}");
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

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象インスタンスを取得
        Assert.Single(targetInstances);
        var targetInstance = targetInstances.First() as IItemMethod;

        // 型タイプの確認
        Assert.Equal("void", GetExpressions(targetInstance.MethodTypes));

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
          ( "str","string","",""),
          ( "intger","int","",""),
          ( "f","float","",""),
          ( "d","decimal","",""),
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));

        // 内部処理の確認
        Assert.Empty(targetInstance.Members);
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

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象インスタンスを取得
        Assert.Single(targetInstances);
        var targetInstance = targetInstances.First() as IItemMethod;

        // 型タイプの確認
        Assert.Equal("void", GetExpressions(targetInstance.MethodTypes));

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
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));

        // 内部処理の確認
        Assert.Empty(targetInstance.Members);
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

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象インスタンスを取得
        Assert.Single(targetInstances);
        var targetInstance = targetInstances.First() as IItemMethod;

        // 型タイプの確認
        Assert.Equal("void", GetExpressions(targetInstance.MethodTypes));

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
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));

        // 内部処理の確認
        Assert.Empty(targetInstance.Members);
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

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象インスタンスを取得
        Assert.True(targetInstances.Count == 2, $"targetInstances != 2 [{targetInstances.Count}]");

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
        foreach (IItemMethod targetInstance in targetInstances)
        {
          // 型タイプの確認
          Assert.Equal("void", GetExpressions(targetInstance.MethodTypes));

          // パラメータの確認
          var expectedModifiers = expectedModifiersList[expectedIndex];
          var expectedArgs = expectedArgsList[expectedIndex];
          Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));

          // 内部処理の確認
          Assert.Empty(targetInstance.Members);

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

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象インスタンスを取得
        Assert.Single(targetInstances);
        var targetInstance = targetInstances.First() as IItemMethod;

        // 型タイプの確認
        Assert.Equal("void", GetExpressions(targetInstance.MethodTypes));

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
          ( "integer","int","ref",""),
          ( "str","string","in",""),
          ( "output","decimal","out",""),
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));

        // 内部処理の確認
        Assert.Empty(targetInstance.Members);
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

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象インスタンスを取得
        Assert.Single(targetInstances);
        var targetInstance = targetInstances.First() as IItemMethod;

        // 型タイプの確認
        Assert.Equal("void", GetExpressions(targetInstance.MethodTypes));

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
          ( "integer","int","","10"),
          ( "str","string","","\"ABC\""),
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));

        // 内部処理の確認
        Assert.Empty(targetInstance.Members);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 戻り値のテスト
    /// </summary>
    [Fact(DisplayName = "ReturnValue")]
    public void ReturnValueTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnValue), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnValue.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象インスタンスを取得
        Assert.Single(targetInstances);
        var targetInstance = targetInstances.First() as IItemMethod;

        // 型タイプの確認
        Assert.Equal("int", GetExpressions(targetInstance.MethodTypes));

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>();
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));

        // 内部処理の確認
        Assert.Single(targetInstance.Members);
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
    /// <param name="targetInstance">対象のインスタンス</param>
    /// <param name="modifiers">アクセス修飾子の期待値</param>
    /// <param name="expectedArgs">パラメータの期待値</param>
    /// <returns>条件が一致するメンバー数</returns>
    private int GetMemberCount(IItemMethod targetInstance, List<string> modifiers, List<(string name, string expressions, string refType, string defaultValue)> expectedArgs)
    {
      // アクセス修飾子の確認
      Assert.Equal(modifiers, targetInstance.Modifiers);

      // パラメータ数の確認
      Assert.Equal(expectedArgs.Count, targetInstance.Args.Count);

      // パラメータの確認
      var argCount = 0;
      foreach (var (name, expressions, refType, defaultValue) in expectedArgs)
      {
        var actualArgs = targetInstance.Args
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
