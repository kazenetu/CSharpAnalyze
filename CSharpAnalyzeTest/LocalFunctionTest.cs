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
  [Trait("ローカルメソッドのテスト", nameof(LocalFunctionTest))]
  public class LocalFunctionTest : TestBase
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
      LambdaReturn,
      LambdaVoid,
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

          source.Add("void target()");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.StandardArgs:
          filePath = "StandardArgs.cs";

          source.Add("void target(string str,int intger,float f,decimal d)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.ClassArgs:
          filePath = "ClassArgs.cs";

          source.Add("void target(Standard instance)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.ListArgs:
          filePath = "ListArgs.cs";

          source.Add("void target(List<string> list)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.Multiple:
          filePath = "Multiple.cs";

          source.Add("void target(string str)");
          source.Add("{");
          source.Add("}");
          source.Add("void target(string str1,int integer)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.RefArgs:
          filePath = "RefArgs.cs";

          source.Add("void target(ref int integer,in string str,out decimal output)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.DefaultValues:
          filePath = "DefaultValues.cs";

          source.Add("void target(int integer=10,string str=\"ABC\")");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.ReturnValue:
          filePath = "ReturnValue.cs";

          source.Add("int target()");
          source.Add("{");
          source.Add("  return 10;");
          source.Add("}");
          break;

        case CreatePattern.LambdaReturn:
          filePath = "LambdaReturn.cs";

          source.Add("int target()=>10;target();");
          break;

        case CreatePattern.LambdaVoid:
          filePath = "LambdaVoid.cs";

          source.Add("void target() => System.Diagnostics.Debug.WriteLine(10);target();");
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
    public LocalFunctionTest() : base()
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

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemLocalFunction;

        // 型タイプの確認
        Assert.Equal("void", GetExpressionsToString(targetInstance.MethodTypes));

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // パラメータの確認
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedArgs));

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

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemLocalFunction;

        // 型タイプの確認
        Assert.Equal("void", GetExpressionsToString(targetInstance.MethodTypes));

        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);
        Assert.Equal("Standard", ev.FileRoot.OtherFiles.First().Key);
        Assert.Equal("Standard.cs", ev.FileRoot.OtherFiles.First().Value);

        // パラメータの確認
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
          ( "instance","Standard","",""),
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedArgs));

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

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemLocalFunction;

        // 型タイプの確認
        Assert.Equal("void", GetExpressionsToString(targetInstance.MethodTypes));

        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);
        Assert.Equal("List", ev.FileRoot.OtherFiles.First().Key);
        Assert.Equal("", ev.FileRoot.OtherFiles.First().Value);

        // パラメータの確認
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
          ( "list","List<string>","",""),
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedArgs));

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
        var targetParentInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetParentInstances);
        var targetParentInstance = targetParentInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstances = GetTargetInstances(targetParentInstance);

        // 対象インスタンスを取得
        Assert.True(targetInstances.Count == 2, $"targetInstances != 2 [{targetInstances.Count}]");

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // クラス内の要素の想定値を設定
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
        foreach (IItemLocalFunction targetInstance in targetInstances)
        {
          // 型タイプの確認
          Assert.Equal("void", GetExpressionsToString(targetInstance.MethodTypes));

          // パラメータの確認
          var expectedArgs = expectedArgsList[expectedIndex];
          Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedArgs));

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

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemLocalFunction;

        // 型タイプの確認
        Assert.Equal("void", GetExpressionsToString(targetInstance.MethodTypes));

        // パラメータの確認
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
          ( "integer","int","ref",""),
          ( "str","string","in",""),
          ( "output","decimal","out",""),
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedArgs));

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

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemLocalFunction;

        // 型タイプの確認
        Assert.Equal("void", GetExpressionsToString(targetInstance.MethodTypes));

        // パラメータの確認
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
          ( "integer","int","","10"),
          ( "str","string","","\"ABC\""),
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedArgs));

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

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemLocalFunction;

        // 型タイプの確認
        Assert.Equal("int", GetExpressionsToString(targetInstance.MethodTypes));

        // パラメータの確認
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>();
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedArgs));

        // 内部処理の確認
        Assert.Single(targetInstance.Members);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// ラムダ 戻り値テスト
    /// </summary>
    [Fact(DisplayName = "LambdaReturn")]
    public void LambdaReturnTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.LambdaReturn), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "LambdaReturn.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemLocalFunction;

        // 型タイプの確認
        Assert.Equal("int", GetExpressionsToString(targetInstance.MethodTypes));

        // パラメータの確認
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>();
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedArgs));

        // 内部処理の確認
        Assert.Single(targetInstance.Members);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// ラムダ 戻り値なしテスト
    /// </summary>
    [Fact(DisplayName = "LambdaVoid")]
    public void LambdaVoidTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.LambdaVoid), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "LambdaVoid.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemLocalFunction;

        // 型タイプの確認
        Assert.Equal("void", GetExpressionsToString(targetInstance.MethodTypes));

        // パラメータの確認
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>();
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedArgs));

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
    /// 対象インスタンスの取得
    /// </summary>
    /// <param name="itemClass">対象のアイテムメソッド</param>
    /// <returns>対象インスタンスリスト</returns>
    private List<IItemLocalFunction> GetTargetInstances(IItemMethod itemMethod)
    {
      return itemMethod.Members.Where(member => member is IItemLocalFunction).
              Select(member => member as IItemLocalFunction).ToList();
    }

    /// <summary>
    /// メンバー数を取得
    /// </summary>
    /// <param name="targetInstance">対象のインスタンス</param>
    /// <param name="expectedArgs">パラメータの期待値</param>
    /// <returns>条件が一致するメンバー数</returns>
    private int GetMemberCount(IItemLocalFunction targetInstance,  List<(string name, string expressions, string refType, string defaultValue)> expectedArgs)
    {
      // パラメータ数の確認
      Assert.Equal(expectedArgs.Count, targetInstance.Args.Count);

      // パラメータの確認
      var argCount = 0;
      foreach (var (name, expressions, refType, defaultValue) in expectedArgs)
      {
        var actualArgs = targetInstance.Args
                        .Where(arg => arg.name == name)
                        .Where(arg => GetExpressionsToString(arg.expressions) == expressions)
                        .Where(arg => string.Concat(arg.modifiers) == refType)
                        .Where(arg => GetExpressionsToString(arg.defaultValues) == defaultValue);
        if (actualArgs.Any())
        {
          argCount++;
        }
      }
      return argCount;
    }

  }
}
