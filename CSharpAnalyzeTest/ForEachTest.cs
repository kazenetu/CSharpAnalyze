﻿using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyzeTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  [Trait("ForEachのテスト", nameof(ForEachTest))]
  public class ForEachTest : TestBase
  {
    /// <summary>
    /// ソース作成パターン
    /// </summary>
    private enum CreatePattern
    {
      Standard,
      ListStrings,
      DictionaryKey,
      DictionaryValue,
      Dictionary,
      GenericsNest,
      Indexer,
      InstanceProperty,
      InstanceMethod,
      InnerClassMethod,
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
      var addMember = new List<string>();
      var addSource = new StringBuilder();

      switch (pattern)
      {
        case CreatePattern.Standard:
          filePath = "Standard.cs";

          source.Add("var values = new int[]{1,2,3 };");
          source.Add("foreach(var value in values)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.ListStrings:
          filePath = "ListStrings.cs";

          source.Add("var values = new List<string>{\"1\",\"2\", \"3\" };");
          source.Add("foreach(var value in values)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.DictionaryKey:
          filePath = "DictionaryKey.cs";

          source.Add("var values = new Dictionary<int,string>{{10,\"1\"},{20,\"2\"}, {30,\"3\" }};");
          source.Add("foreach(var value in values.Keys)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.DictionaryValue:
          filePath = "DictionaryValue.cs";

          source.Add("var values = new Dictionary<int,string>{{10,\"1\"},{20,\"2\"}, {30,\"3\" }};");
          source.Add("foreach(var value in values.Values)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.Dictionary:
          filePath = "Dictionary.cs";

          source.Add("var values = new Dictionary<int,string>{{10,\"1\"},{20,\"2\"}, {30,\"3\" }};");
          source.Add("foreach(var value in values)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.GenericsNest:
          filePath = "GenericsNest.cs";

          source.Add("var values = new Dictionary<int,List<string>>");
          source.Add("{");
          source.Add("{0,new List<string>{\"a\",\"b\",\"c\",} },");
          source.Add("{1,new List<string>{\"A\",\"B\",\"C\",} },");
          source.Add("};");
          source.Add("foreach(var value in values)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.Indexer:
          filePath = "Indexer.cs";

          source.Add("var values = new Dictionary<int,List<string>>");
          source.Add("{");
          source.Add("{0,new List<string>{\"a\",\"b\",\"c\",} },");
          source.Add("{1,new List<string>{\"A\",\"B\",\"C\",} },");
          source.Add("};");
          source.Add("foreach(var value in values[0])");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.InstanceProperty:
          filePath = "InstanceProperty.cs";

          addMember.Add("private List<int> Values{get;}= new List<int>();");

          source.Add("Values.AddRange(new List<int>{0,1,2,3});");
          source.Add("foreach(var value in Values)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.InstanceMethod:
          filePath = "InstanceMethod.cs";

          addMember.Add("private List<int> GetValues()");
          addMember.Add("{");
          addMember.Add("  return new List<int>{0,1,2,3};");
          addMember.Add("{");

          source.Add("foreach(var value in GetValues())");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.InnerClassMethod:
          filePath = "InnerClassMethod.cs";

          addMember.Add("public class InnerClass");
          addMember.Add("{");
          addMember.Add("}");
          addMember.Add("private List<Standard.InnerClass> GetValues()");
          addMember.Add("{");
          addMember.Add("  return new List<Standard.InnerClass>();");
          addMember.Add("{");

          source.Add("foreach(var value in GetValues())");
          source.Add("{");
          source.Add("}");
          break;
      }

      var targetSource = new StringBuilder();
      targetSource.AppendLine("public class Standard");
      targetSource.AppendLine("{");
      targetSource.AppendLine("  public void TestMethod()");
      targetSource.AppendLine("  {");
      source.ForEach(line => targetSource.AppendLine($"    {line }"));
      targetSource.AppendLine("  }");

      addMember.ForEach(line => targetSource.AppendLine($"  {line }"));
      targetSource.AppendLine("}");
      targetSource.AppendLine(addSource.ToString());

      return new FileData(filePath, usingList.ToString(), targetSource.ToString());
    }

    /// <summary>
    /// Setup
    /// </summary>
    public ForEachTest() : base()
    {
      System.Diagnostics.Debug.WriteLine($"Setup {Environment.CurrentDirectory}");
    }

    /// <summary>
    /// 基本的なテスト
    /// </summary>
    [Fact(DisplayName = "Standard")]
    public void StandardTest()
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
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemForEach;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // ローカル部の型の確認
        Assert.Equal("int", GetExpressionsToString(targetInstance.LocalTypes));

        // ローカル部の確認
        Assert.Equal("value", GetExpressionsToString(targetInstance.Local));

        // コレクション部の型の確認
        Assert.Equal("int[]", GetExpressionsToString(targetInstance.CollectionTypes));

        // コレクション部の確認
        Assert.Equal("values", GetExpressionsToString(targetInstance.Collection));

      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// Listジェネリクスのテスト
    /// </summary>
    [Fact(DisplayName = "ListStrings")]
    public void ListStringsTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ListStrings), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ListStrings.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemForEach;

        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);
        Assert.Equal("List", ev.FileRoot.OtherFiles.First().Key);

        // ローカル部の型の確認
        Assert.Equal("string", GetExpressionsToString(targetInstance.LocalTypes));

        // ローカル部の確認
        Assert.Equal("value", GetExpressionsToString(targetInstance.Local));

        // コレクション部の型の確認
        Assert.Equal("List<string>", GetExpressionsToString(targetInstance.CollectionTypes));

        // コレクション部の確認
        Assert.Equal("values", GetExpressionsToString(targetInstance.Collection));

      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// Dictionaryジェネリクスのテスト:Keys
    /// </summary>
    [Fact(DisplayName = "DictionaryKey")]
    public void DictionaryKeyTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.DictionaryKey), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "DictionaryKey.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemForEach;

        // 外部参照の存在確認
        var expectedClassName = new List<string> { "KeyCollection", "Dictionary" };
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

        // ローカル部の型の確認
        Assert.Equal("int", GetExpressionsToString(targetInstance.LocalTypes));

        // ローカル部の確認
        Assert.Equal("value", GetExpressionsToString(targetInstance.Local));

        // コレクション部の型の確認
        Assert.Equal("Dictionary<int,string>.KeyCollection", GetExpressionsToString(targetInstance.CollectionTypes));

        // コレクション部の確認
        Assert.Equal("values.Keys", GetExpressionsToString(targetInstance.Collection));

      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// Dictionaryジェネリクスのテスト:Values
    /// </summary>
    [Fact(DisplayName = "DictionaryValue")]
    public void DictionaryValueTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.DictionaryValue), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "DictionaryValue.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemForEach;

        // 外部参照の存在確認
        var expectedClassName = new List<string> { "ValueCollection", "Dictionary" };
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

        // ローカル部の型の確認
        Assert.Equal("string", GetExpressionsToString(targetInstance.LocalTypes));

        // ローカル部の確認
        Assert.Equal("value", GetExpressionsToString(targetInstance.Local));

        // コレクション部の型の確認
        Assert.Equal("Dictionary<int,string>.ValueCollection", GetExpressionsToString(targetInstance.CollectionTypes));

        // コレクション部の確認
        Assert.Equal("values.Values", GetExpressionsToString(targetInstance.Collection));

      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// Dictionaryジェネリクスのテスト:Dictionary
    /// </summary>
    [Fact(DisplayName = "Dictionary")]
    public void DictionaryTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Dictionary), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Dictionary.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemForEach;

        // 外部参照の存在確認
        var expectedClassName = new List<string> { "Dictionary" };
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

        // ローカル部の型の確認
        Assert.Equal("KeyValuePair<int,string>", GetExpressionsToString(targetInstance.LocalTypes));

        // ローカル部の確認
        Assert.Equal("value", GetExpressionsToString(targetInstance.Local));

        // コレクション部の型の確認
        Assert.Equal("Dictionary<int,string>", GetExpressionsToString(targetInstance.CollectionTypes));

        // コレクション部の確認
        Assert.Equal("values", GetExpressionsToString(targetInstance.Collection));

      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// ジェネリクスのネストのテスト
    /// </summary>
    [Fact(DisplayName = "GenericsNest")]
    public void GenericsNestTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.GenericsNest), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "GenericsNest.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemForEach;

        // 外部参照の存在確認
        var expectedClassName = new List<string> { "Dictionary", "List" };
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

        // ローカル部の型の確認
        Assert.Equal("KeyValuePair<int,List<string>>", GetExpressionsToString(targetInstance.LocalTypes));

        // ローカル部の確認
        Assert.Equal("value", GetExpressionsToString(targetInstance.Local));

        // コレクション部の型の確認
        Assert.Equal("Dictionary<int,List<string>>", GetExpressionsToString(targetInstance.CollectionTypes));

        // コレクション部の確認
        Assert.Equal("values", GetExpressionsToString(targetInstance.Collection));

      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// インデクサのテスト
    /// </summary>
    [Fact(DisplayName = "Indexer")]
    public void IndexerTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Indexer), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Indexer.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemForEach;

        // 外部参照の存在確認
        var expectedClassName = new List<string> { "Dictionary", "List" };
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

        // ローカル部の型の確認
        Assert.Equal("string", GetExpressionsToString(targetInstance.LocalTypes));

        // ローカル部の確認
        Assert.Equal("value", GetExpressionsToString(targetInstance.Local));

        // コレクション部の型の確認
        Assert.Equal("List<string>", GetExpressionsToString(targetInstance.CollectionTypes));

        // コレクション部の確認
        Assert.Equal("values[0]", GetExpressionsToString(targetInstance.Collection));

      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// プロパティ参照のテスト
    /// </summary>
    [Fact(DisplayName = "InstanceProperty")]
    public void InstancePropertyTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.InstanceProperty), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "InstanceProperty.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemForEach;

        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);
        Assert.Equal("List", ev.FileRoot.OtherFiles.First().Key);

        // ローカル部の型の確認
        Assert.Equal("int", GetExpressionsToString(targetInstance.LocalTypes));

        // ローカル部の確認
        Assert.Equal("value", GetExpressionsToString(targetInstance.Local));

        // コレクション部の型の確認
        Assert.Equal("List<int>", GetExpressionsToString(targetInstance.CollectionTypes));

        // コレクション部の確認
        Assert.Equal("this.Values", GetExpressionsToString(targetInstance.Collection));

      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// メソッド参照のテスト
    /// </summary>
    [Fact(DisplayName = "InstanceMethod")]
    public void InstanceMethodTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.InstanceMethod), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "InstanceMethod.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Equal(2, targetInstances.Count);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemForEach;

        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);
        Assert.Equal("List", ev.FileRoot.OtherFiles.First().Key);

        // ローカル部の型の確認
        Assert.Equal("int", GetExpressionsToString(targetInstance.LocalTypes));

        // ローカル部の確認
        Assert.Equal("value", GetExpressionsToString(targetInstance.Local));

        // コレクション部の型の確認
        Assert.Equal("List<int>", GetExpressionsToString(targetInstance.CollectionTypes));

        // コレクション部の確認
        Assert.Equal("this.GetValues()", GetExpressionsToString(targetInstance.Collection));

      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 内部クラスのメソッド参照のテスト
    /// </summary>
    [Fact(DisplayName = "InnerClassMethod")]
    public void InnerClassMethodTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.InnerClassMethod), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "InnerClassMethod.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Equal(2, targetInstances.Count);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemForEach;

        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);
        Assert.Equal("List", ev.FileRoot.OtherFiles.First().Key);

        // ローカル部の型の確認
        Assert.Equal("Standard.InnerClass", GetExpressionsToString(targetInstance.LocalTypes));

        // ローカル部の確認
        Assert.Equal("value", GetExpressionsToString(targetInstance.Local));

        // コレクション部の型の確認
        Assert.Equal("List<Standard.InnerClass>", GetExpressionsToString(targetInstance.CollectionTypes));

        // コレクション部の確認
        Assert.Equal("this.GetValues()", GetExpressionsToString(targetInstance.Collection));

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
    private List<IItemForEach> GetTargetInstances(IItemMethod itemMethod)
    {
      return itemMethod.Members.Where(member => member is IItemForEach).
              Select(member => member as IItemForEach).ToList();
    }
    #endregion
  }
}
