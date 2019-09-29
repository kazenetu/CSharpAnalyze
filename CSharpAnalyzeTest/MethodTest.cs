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
      LambdaReturn,
      LambdaVoid,
      Generics,
      TempInnerClass,
      InnerClassArgs,
      EnumArgs,
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

        case CreatePattern.LambdaReturn:
          filePath = "LambdaReturn.cs";

          source.AppendLine("public class LambdaReturn");
          source.AppendLine("{");
          source.AppendLine("  public int TestMethod() => 10;");
          source.AppendLine("}");
          break;

        case CreatePattern.LambdaVoid:
          filePath = "LambdaVoid.cs";

          source.AppendLine("public class LambdaVoid");
          source.AppendLine("{");
          source.AppendLine("  public void TestMethod() => System.Diagnostics.Debug.WriteLine(10);");
          source.AppendLine("}");
          break;

        case CreatePattern.Generics:
          filePath = "Generics.cs";

          source.AppendLine("public class Generics");
          source.AppendLine("{");
          source.AppendLine("  public void TestMethod<T,V>()");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.TempInnerClass:
          filePath = "TempInnerClass.cs";

          source.AppendLine("public class TempInnerClass");
          source.AppendLine("{");
          source.AppendLine("  public class InnerClass");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.InnerClassArgs:
          filePath = "InnerClassArgs.cs";

          source.AppendLine("public class InnnerClassArgs");
          source.AppendLine("{");
          source.AppendLine("  public TempInnerClass.InnerClass TestMethod(TempInnerClass.InnerClass instance = new TempInnerClass.InnerClass())");
          source.AppendLine("  {");
          source.AppendLine("    return instance;");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.EnumArgs:
          filePath = "EnumArgs.cs";

          source.AppendLine("public class Standard");
          source.AppendLine("{");
          source.AppendLine("  prvate enum EnumTest");
          source.AppendLine("  {");
          source.AppendLine("    Test");
          source.AppendLine("  }");
          source.AppendLine("  public EnumTest TestMethod(EnumTest enumArg = Standard.EnumTest.Test)");
          source.AppendLine("  {");
          source.AppendLine("    return Enum.Test;");
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

        //ジェネリックの確認
        var expectedGenericTypes = new List<string>();
        Assert.Equal(expectedGenericTypes, targetInstance.GenericTypes);

        // 型タイプの確認
        Assert.Equal("void", GetExpressionsToString(targetInstance.MethodTypes));

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
        // 期待値数と一致要素数の確認
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));
        // 実際の要素数との一致確認
        Assert.Equal(expectedArgs.Count, targetInstance.Args.Count);

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

        //ジェネリックの確認
        var expectedGenericTypes = new List<string>();
        Assert.Equal(expectedGenericTypes, targetInstance.GenericTypes);

        // 型タイプの確認
        Assert.Equal("void", GetExpressionsToString(targetInstance.MethodTypes));

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
        // 期待値数と一致要素数の確認
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));
        // 実際の要素数との一致確認
        Assert.Equal(expectedArgs.Count, targetInstance.Args.Count);

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

        //ジェネリックの確認
        var expectedGenericTypes = new List<string>();
        Assert.Equal(expectedGenericTypes, targetInstance.GenericTypes);

        // 型タイプの確認
        Assert.Equal("void", GetExpressionsToString(targetInstance.MethodTypes));

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
        // 期待値数と一致要素数の確認
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));
        // 実際の要素数との一致確認
        Assert.Equal(expectedArgs.Count, targetInstance.Args.Count);

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
          //ジェネリックの確認
          var expectedGenericTypes = new List<string>();
          Assert.Equal(expectedGenericTypes, targetInstance.GenericTypes);

          // 型タイプの確認
          Assert.Equal("void", GetExpressionsToString(targetInstance.MethodTypes));

          // パラメータの確認
          var expectedModifiers = expectedModifiersList[expectedIndex];
          var expectedArgs = expectedArgsList[expectedIndex];
          // 期待値数と一致要素数の確認
          Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));
          // 実際の要素数との一致確認
          Assert.Equal(expectedArgs.Count, targetInstance.Args.Count);

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

        //ジェネリックの確認
        var expectedGenericTypes = new List<string>();
        Assert.Equal(expectedGenericTypes, targetInstance.GenericTypes);

        // 型タイプの確認
        Assert.Equal("void", GetExpressionsToString(targetInstance.MethodTypes));

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
          ( "integer","int","ref",""),
          ( "str","string","in",""),
          ( "output","decimal","out",""),
        };
        // 期待値数と一致要素数の確認
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));
        // 実際の要素数との一致確認
        Assert.Equal(expectedArgs.Count, targetInstance.Args.Count);

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

        //ジェネリックの確認
        var expectedGenericTypes = new List<string>();
        Assert.Equal(expectedGenericTypes, targetInstance.GenericTypes);

        // 型タイプの確認
        Assert.Equal("void", GetExpressionsToString(targetInstance.MethodTypes));

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
          ( "integer","int","","10"),
          ( "str","string","","\"ABC\""),
        };
        // 期待値数と一致要素数の確認
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));
        // 実際の要素数との一致確認
        Assert.Equal(expectedArgs.Count, targetInstance.Args.Count);

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

        //ジェネリックの確認
        var expectedGenericTypes = new List<string>();
        Assert.Equal(expectedGenericTypes, targetInstance.GenericTypes);

        // 型タイプの確認
        Assert.Equal("int", GetExpressionsToString(targetInstance.MethodTypes));

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>();
        // 期待値数と一致要素数の確認
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));
        // 実際の要素数との一致確認
        Assert.Equal(expectedArgs.Count, targetInstance.Args.Count);

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

        // 対象インスタンスを取得
        Assert.Single(targetInstances);
        var targetInstance = targetInstances.First() as IItemMethod;

        //ジェネリックの確認
        var expectedGenericTypes = new List<string>();
        Assert.Equal(expectedGenericTypes, targetInstance.GenericTypes);

        // 型タイプの確認
        Assert.Equal("int", GetExpressionsToString(targetInstance.MethodTypes));

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>();
        // 期待値数と一致要素数の確認
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));
        // 実際の要素数との一致確認
        Assert.Equal(expectedArgs.Count, targetInstance.Args.Count);

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

        // 対象インスタンスを取得
        Assert.Single(targetInstances);
        var targetInstance = targetInstances.First() as IItemMethod;

        //ジェネリックの確認
        var expectedGenericTypes = new List<string>();
        Assert.Equal(expectedGenericTypes, targetInstance.GenericTypes);

        // 型タイプの確認
        Assert.Equal("void", GetExpressionsToString(targetInstance.MethodTypes));

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>();
        // 期待値数と一致要素数の確認
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));
        // 実際の要素数との一致確認
        Assert.Equal(expectedArgs.Count, targetInstance.Args.Count);

        // 内部処理の確認
        Assert.Single(targetInstance.Members);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// ジェネリックテスト
    /// </summary>
    [Fact(DisplayName = "Generics")]
    public void GenericsTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Generics), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Generics.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象インスタンスを取得
        Assert.Single(targetInstances);
        var targetInstance = targetInstances.First() as IItemMethod;

        //ジェネリックの確認
        var expectedGenericTypes = new List<string>() { "T", "V" };
        Assert.Equal(expectedGenericTypes, targetInstance.GenericTypes);

        // 型タイプの確認
        Assert.Equal("void", GetExpressionsToString(targetInstance.MethodTypes));

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>();
        // 期待値数と一致要素数の確認
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));
        // 実際の要素数との一致確認
        Assert.Equal(expectedArgs.Count, targetInstance.Args.Count);

        // 内部処理の確認
        Assert.Empty(targetInstance.Members);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 内部クラスインスタンスのパラメータのテスト
    /// </summary>
    [Fact(DisplayName = "InnerClassArgs")]
    public void InnerClassArgsTest()
    {
      // 内部クラステストコードを追加
      CreateFileData(CreateSource(CreatePattern.TempInnerClass), null);

      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.InnerClassArgs), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "InnerClassArgs.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象インスタンスを取得
        Assert.Single(targetInstances);
        var targetInstance = targetInstances.First() as IItemMethod;

        //ジェネリックの確認
        var expectedGenericTypes = new List<string>();
        Assert.Equal(expectedGenericTypes, targetInstance.GenericTypes);

        // 型タイプの確認
        Assert.Equal("TempInnerClass.InnerClass", GetExpressionsToString(targetInstance.MethodTypes));

        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);
        Assert.Equal("TempInnerClass.InnerClass", ev.FileRoot.OtherFiles.First().Key);
        Assert.Equal("TempInnerClass.cs", ev.FileRoot.OtherFiles.First().Value);

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
          ( "instance","TempInnerClass.InnerClass","","new TempInnerClass.InnerClass()"),
        };
        // 期待値数と一致要素数の確認
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));
        // 実際の要素数との一致確認
        Assert.Equal(expectedArgs.Count, targetInstance.Args.Count);

        // 内部処理の確認
        Assert.Single(targetInstance.Members);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 内部クラスインスタンスのパラメータのテスト
    /// </summary>
    [Fact(DisplayName = "EnumArgsTest")]
    public void EnumArgsTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.EnumArgs), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "EnumArgs.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象インスタンスを取得
        Assert.Single(targetInstances);
        var targetInstance = targetInstances.First() as IItemMethod;

        //ジェネリックの確認
        var expectedGenericTypes = new List<string>();
        Assert.Equal(expectedGenericTypes, targetInstance.GenericTypes);

        // 型タイプの確認
        Assert.Equal("Standard.EnumTest", GetExpressionsToString(targetInstance.MethodTypes));

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // パラメータの確認
        var expectedModifiers = new List<string>() { "public" };
        var expectedArgs = new List<(string name, string expressions, string refType, string defaultValue)>()
        {
          ( "enumArg","Standard.EnumTest","","Standard.EnumTest.Test"),
        };
        // 期待値数と一致要素数の確認
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetInstance, expectedModifiers, expectedArgs));
        // 実際の要素数との一致確認
        Assert.Equal(expectedArgs.Count, targetInstance.Args.Count);

        // 内部処理の確認
        Assert.Single(targetInstance.Members);
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
    #endregion

  }
}
