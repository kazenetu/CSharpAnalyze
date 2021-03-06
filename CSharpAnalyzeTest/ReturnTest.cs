﻿using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyzeTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  [Trait("Returnのテスト", nameof(ReturnTest))]
  public class ReturnTest : TestBase
  {
    /// <summary>
    /// ソース作成パターン
    /// </summary>
    private enum CreatePattern
    {
      ReturnVoid,
      ReturnInt,
      ReturnString,
      ReturnInstance,
      ReturnGenericInstance,
      ReturnLocalDeclaration,
      ReturnLocalFunction,
      ReturnMethod,
      ReturnClassMethod,
      ReturnClassProperty,
      ReturnInnerClassMethod,
      ReturnExpression,
      ReturnExpressionChangedPriority,
      ReturnExpressionChangedPriority2,
      ReturnIntTostring,
      ReturnAddIntTostring,
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
        case CreatePattern.ReturnVoid:
          filePath = "ReturnVoid.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public void TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    return;");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.ReturnInt:
          filePath = "ReturnInt.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public int TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    return 10;");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.ReturnString:
          filePath = "ReturnString.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public string TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    return \"ABC\";");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.ReturnInstance:
          filePath = "ReturnInstance.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public Test TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    return new Test();");
          source.AppendLine("  }");
          source.AppendLine("}");
          source.AppendLine("public class Test");
          source.AppendLine("{");
          source.AppendLine("}");
          break;

        case CreatePattern.ReturnGenericInstance:
          filePath = "ReturnGenericInstance.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public List<Test> TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    return new List<Test>();");
          source.AppendLine("  }");
          source.AppendLine("}");
          source.AppendLine("public class Test");
          source.AppendLine("{");
          source.AppendLine("}");
          break;

        case CreatePattern.ReturnLocalDeclaration:
          filePath = "ReturnLocalDeclaration.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public string TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    var result = \"ABC\";");
          source.AppendLine("    return result;");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.ReturnLocalFunction:
          filePath = "ReturnLocalFunction.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public int TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    int target()=>10;");
          source.AppendLine("    return target();");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.ReturnMethod:
          filePath = "ReturnMethod.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public int TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    return Target(1);");
          source.AppendLine("  }");
          source.AppendLine("");
          source.AppendLine("  public int Target(int value)");
          source.AppendLine("  {");
          source.AppendLine("    return value;");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.ReturnClassMethod:
          filePath = "ReturnClassMethod.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public int TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    return Target(1);");
          source.AppendLine("  }");
          source.AppendLine("");
          source.AppendLine("  public static int Target(int value)");
          source.AppendLine("  {");
          source.AppendLine("    return value;");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.ReturnClassProperty:
          filePath = "ReturnClassProperty.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public int TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    return Target;");
          source.AppendLine("  }");
          source.AppendLine("");
          source.AppendLine("  public static int Target{get;} = 10:");
          source.AppendLine("}");
          break;

        case CreatePattern.ReturnInnerClassMethod:
          filePath = "ReturnInnerClassMethod.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public int TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    return InnerClass.Target(1);");
          source.AppendLine("  }");
          source.AppendLine("");
          source.AppendLine("  public class InnerClass");
          source.AppendLine("  {");
          source.AppendLine("    public static int Target(int value)");
          source.AppendLine("    {");
          source.AppendLine("      return value;");
          source.AppendLine("    }");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.ReturnExpression:
          filePath = "ReturnExpression.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public int TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    return 1+2*3;");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.ReturnExpressionChangedPriority:
          filePath = "ReturnExpressionChangedPriority.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public int TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    return (1+2)*3;");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.ReturnExpressionChangedPriority2:
          filePath = "ReturnExpressionChangedPriority2.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public int TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    return 1+(2*3);");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.ReturnIntTostring:
          filePath = "ReturnIntTostring.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public string TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    return (10).ToString();");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.ReturnAddIntTostring:
          filePath = "ReturnAddIntTostring.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public string TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    return (10+100).ToString();");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;
      }

      return new FileData(filePath, usingList.ToString(), source.ToString());
    }

    /// <summary>
    /// Setup
    /// </summary>
    public ReturnTest() : base()
    {
      System.Diagnostics.Debug.WriteLine($"Setup {Environment.CurrentDirectory}");
    }

    /// <summary>
    /// 値を返さないテスト
    /// </summary>
    [Fact(DisplayName = "ReturnVoid")]
    public void ReturnVoidTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnVoid), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnVoid.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("", GetExpressionsToString(targetInstance.ReturnValue));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// int値を返すテスト
    /// </summary>
    [Fact(DisplayName = "ReturnInt")]
    public void ReturnIntTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnInt), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnInt.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("10", GetExpressionsToString(targetInstance.ReturnValue));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// string値を返すテスト
    /// </summary>
    [Fact(DisplayName = "ReturnString")]
    public void ReturnStringTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnString), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnString.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("\"ABC\"", GetExpressionsToString(targetInstance.ReturnValue));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// クラスインスタンスを返すテスト
    /// </summary>
    [Fact(DisplayName = "ReturnInstance")]
    public void ReturnInstanceTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnInstance), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnInstance.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("new Test()", GetExpressionsToString(targetInstance.ReturnValue));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// Listインスタンスを返すテスト
    /// </summary>
    [Fact(DisplayName = "ReturnGenericInstance")]
    public void ReturnGenericInstanceTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnGenericInstance), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnGenericInstance.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);
        Assert.Equal("List", ev.FileRoot.OtherFiles.First().Key);
        Assert.Equal("", ev.FileRoot.OtherFiles.First().Value);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("new List<Test>()", GetExpressionsToString(targetInstance.ReturnValue));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// ローカル変数を返すテスト
    /// </summary>
    [Fact(DisplayName = "ReturnLocalDeclaration")]
    public void ReturnLocalDeclarationTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnLocalDeclaration), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnLocalDeclaration.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("result", GetExpressionsToString(targetInstance.ReturnValue));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// ローカルメソッドの戻り値を返すテスト
    /// </summary>
    [Fact(DisplayName = "ReturnLocalFunction")]
    public void ReturnLocalFunctionTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnLocalFunction), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnLocalFunction.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("target()", GetExpressionsToString(targetInstance.ReturnValue));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// メソッドの戻り値を返すテスト
    /// </summary>
    [Fact(DisplayName = "ReturnMethod")]
    public void ReturnMethodTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnMethod), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnMethod.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Equal(2, targetInstances.Count);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("this.Target(1)", GetExpressionsToString(targetInstance.ReturnValue));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// クラスメソッドの戻り値を返すテスト
    /// </summary>
    [Fact(DisplayName = "ReturnClassMethod")]
    public void ReturnClassMethodTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnClassMethod), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnClassMethod.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Equal(2, targetInstances.Count);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("ReturnValue.Target(1)", GetExpressionsToString(targetInstance.ReturnValue));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// クラスプロパティを返すテスト
    /// </summary>
    [Fact(DisplayName = "ReturnClassProperty")]
    public void ReturnClassPropertyTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnClassProperty), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnClassProperty.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("ReturnValue.Target", GetExpressionsToString(targetInstance.ReturnValue));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 内部クラスのクラスメソッドの戻り値を返すテスト
    /// </summary>
    [Fact(DisplayName = "ReturnInnerClassMethod")]
    public void ReturnInnerClassMethodTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnInnerClassMethod), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnInnerClassMethod.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("ReturnValue.InnerClass.Target(1)", GetExpressionsToString(targetInstance.ReturnValue));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 式を返すテスト
    /// </summary>
    [Fact(DisplayName = "ReturnExpression")]
    public void ReturnExpressionTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnExpression), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnExpression.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("1+2*3", GetExpressionsToString(targetInstance.ReturnValue));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// かっこを利用した式を返すテスト
    /// </summary>
    [Fact(DisplayName = "ReturnExpressionChangedPriority")]
    public void ReturnExpressionChangedPriorityTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnExpressionChangedPriority), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnExpressionChangedPriority.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("(1+2)*3", GetExpressionsToString(targetInstance.ReturnValue));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// かっこを利用した式を返すテストその2
    /// </summary>
    [Fact(DisplayName = "ReturnExpressionChangedPriority2")]
    public void ReturnExpressionChangedPriority2Test()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnExpressionChangedPriority2), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnExpressionChangedPriority2.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("1+(2*3)", GetExpressionsToString(targetInstance.ReturnValue));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// int型のToStringを返すテスト
    /// </summary>
    [Fact(DisplayName = "ReturnIntTostring")]
    public void ReturnIntTostringTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnIntTostring), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnIntTostring.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("(10).ToString()", GetExpressionsToString(targetInstance.ReturnValue));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// int型のToStringを返すテスト(計算)
    /// </summary>
    [Fact(DisplayName = "ReturnAddIntTostring")]
    public void ReturnAddIntTostringTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnAddIntTostring), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnAddIntTostring.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("(10+100).ToString()", GetExpressionsToString(targetInstance.ReturnValue));
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
    /// 対象インスタンスの取得
    /// </summary>
    /// <param name="itemClass">対象のアイテムメソッド</param>
    /// <returns>対象インスタンスリスト</returns>
    private List<IItemReturn> GetTargetInstances(IItemMethod itemMethod)
    {
      return itemMethod.Members.Where(member => member is IItemReturn).
              Select(member => member as IItemReturn).ToList();
    }

    #endregion
  }
}
