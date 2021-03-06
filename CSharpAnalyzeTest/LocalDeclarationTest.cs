﻿using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyzeTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  [Trait("ローカル変数のテスト", nameof(LocalDeclarationTest))]
  public class LocalDeclarationTest : TestBase
  {
    /// <summary>
    /// ソース作成パターン
    /// </summary>
    private enum CreatePattern
    {
      Standard,
      TypeInference,
      Generics,
      TempInnerClass,
      InnerClass,
      Array,
    }

    /// <summary>
    /// ファイル名、ソースコード取得処理
    /// </summary>
    /// <param name="pattern">生成パターン</param>
    /// <returns>ファイルパスとソースコード</returns>
    private FileData CreateSource(CreatePattern pattern)
    {
      var isCreateDummyClass = true;
      var usingList = new StringBuilder();
      var source = new List<string>();
      var filePath = string.Empty;

      switch (pattern)
      {
        case CreatePattern.Standard:
          filePath = "Standard.cs";

          source.Add("string target;");
          source.Add("int targetInt = 1;");
          break;

        case CreatePattern.TypeInference:
          filePath = "TypeInference.cs";

          source.Add("var target = \"ABC\";");
          source.Add("var targetInt = 1;");
          break;

        case CreatePattern.Generics:
          filePath = "Generics.cs";

          source.Add("List<string> target;");
          source.Add("var targetInt = new List<int>();");
          source.Add("var targetIntDef = new List<int>(){1,2,3};");
          source.Add("var targetDictionary = new Dictionary<int,int>(){{1,2},{3,4}};");
          break;

        case CreatePattern.TempInnerClass:
          isCreateDummyClass = false;

          filePath = "TempInnerClass.cs";

          source.Add("public class TempInnerClass");
          source.Add("{");
          source.Add("  public class InnerClass");
          source.Add("  {");
          source.Add("  }");
          source.Add("}");
          break;

        case CreatePattern.InnerClass:
          filePath = "InnerClass.cs";

          source.Add("var targetClass = new TempInnerClass();");
          source.Add("var targetInnerClass = new TempInnerClass.InnerClass();");
          source.Add("var targetList = new List<TempInnerClass.InnerClass>();");
          source.Add("TempInnerClass.InnerClass[] targetArray = new TempInnerClass.InnerClass[10];");
          break;

        case CreatePattern.Array:
          filePath = "Array.cs";

          source.Add("int[] list = { 1, 2, 3 };");
          break;
      }

      // ソースコード作成
      var targetSource = new StringBuilder();
      if (isCreateDummyClass)
      {
        targetSource.AppendLine("public class Standard");
        targetSource.AppendLine("{");
        targetSource.AppendLine("  public void TestMethod()");
        targetSource.AppendLine("  {");

        source.ForEach(line => targetSource.AppendLine($"    {line}"));

        targetSource.AppendLine("  }");
        targetSource.AppendLine("}");
      }
      else
      {
        source.ForEach(line => targetSource.AppendLine($"{line}"));
      }

      return new FileData(filePath, usingList.ToString(), targetSource.ToString());
    }

    /// <summary>
    /// Setup
    /// </summary>
    public LocalDeclarationTest() : base()
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

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // パラメータの確認
        var expectedArgs = new List<(string type, string name, string defaultValue)>()
        {
          ("string", "target", null),
          ("int", "targetInt", "1")
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetParentInstance, expectedArgs));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 型推論のテスト
    /// </summary>
    [Fact(DisplayName = "TypeInference")]
    public void TypeInferenceTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.TypeInference), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "TypeInference.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // パラメータの確認
        var expectedArgs = new List<(string type, string name, string defaultValue)>()
        {
          ("string", "target", "\"ABC\""),
          ("int", "targetInt", "1")
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetParentInstance, expectedArgs));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// ジェネリックのテスト
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

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        var expectedClassName = new List<string> { "List", "Dictionary" };
        foreach(var fileInfo in ev.FileRoot.OtherFiles)
        {
          // クラス名が一致する場合は予想クラス名リストから対象クラス名を削除
          if (expectedClassName.Contains(fileInfo.Key))
          {
            expectedClassName.Remove(fileInfo.Key);
          }
        }
        // 予想クラス名リストがすべて削除されていることを確認
        Assert.Empty(expectedClassName);

        // パラメータの確認
        var expectedArgs = new List<(string type, string name, string defaultValue)>()
        {
          ("List<string>", "target", null),
          ("List<int>", "targetInt", "new List<int>()"),
          ("List<int>", "targetIntDef", "new List<int>(){1,2,3}"),
          ("Dictionary<int,int>", "targetDictionary", "new Dictionary<int,int>(){{1,2},{3,4}}"),
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetParentInstance, expectedArgs));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 内部クラスインスタンス生成のテスト
    /// </summary>
    [Fact(DisplayName = "InnerClass")]
    public void InnerClassTest()
    {
      // 内部クラステストコードを追加
      CreateFileData(CreateSource(CreatePattern.TempInnerClass), null);

      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.InnerClass), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "InnerClass.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        var expectedClassName = new List<string> { "List", "TempInnerClass.InnerClass", "TempInnerClass" };
        foreach (var fileInfo in ev.FileRoot.OtherFiles)
        {
          // クラス名が一致する場合は予想クラス名リストから対象クラス名を削除
          if (expectedClassName.Contains(fileInfo.Key))
          {
            expectedClassName.Remove(fileInfo.Key);
          }
        }
        // 予想クラス名リストがすべて削除されていることを確認
        Assert.Empty(expectedClassName);

        // パラメータの確認
        var expectedArgs = new List<(string type, string name, string defaultValue)>()
        {
          ("TempInnerClass", "targetClass", "new TempInnerClass()"),
          ("TempInnerClass.InnerClass", "targetInnerClass", "new TempInnerClass.InnerClass()"),
          ("List<TempInnerClass.InnerClass>", "targetList", "new List<TempInnerClass.InnerClass>()"),
          ("TempInnerClass.InnerClass[]", "targetArray", "new TempInnerClass.InnerClass[10]"),
        };
        Assert.Equal(expectedArgs.Count, GetMemberCount(targetParentInstance, expectedArgs));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 配列テスト
    /// </summary>
    [Fact(DisplayName = "ArrayTest")]
    public void ArrayTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Array), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Array.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // パラメータの確認
        var expectedArgs = new List<(string type, string name, string defaultValue)>()
        {
          ("int[]", "list", "[1,2,3]")
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
    private int GetMemberCount(IItemMethod targetParentInstance, List<(string type, string name, string defaultValue)> expectedList)
    {
      var memberCount = 0;
      foreach (var member in targetParentInstance.Members)
      {
        // 対象以外は次のmemberへ
        if (!(member is IItemStatementLocalDeclaration targetMember)) continue;

        // 型の一致確認
        var expectedTargets = expectedList.Where(expected => expected.name == targetMember.Name && expected.type == GetExpressionsToString(targetMember.Types));
        if (!expectedTargets.Any()) continue;

        // 予想値を取得
        var expectedTarget = expectedTargets.First();

        // 初期値の確認
        if (expectedTarget.defaultValue is null)
        {
          Assert.Empty(targetMember.DefaultValues);
        }
        else
        {
          Assert.Equal(expectedTarget.defaultValue, GetExpressionsToString(targetMember.DefaultValues));
        }

        memberCount++;
      }
      return memberCount;
    }
    #endregion

  }
}
