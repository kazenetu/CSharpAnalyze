# CSharpAnalyze
C#のコード解析：ConvertCStoTSの再設計版

# 実行環境
* .NET Core SDK 2.1以上

# 前提
* submoduleなどで他のリポジトリから参照する。

# テスト(ルートで実行)
## ```dotnet test CSharpAnalyzeTest```  

# 実装方法
下記手順で実装する。
1. AnalyzeApplicationのインスタンス作成
1. 解析完了イベントの登録
1. 解析開始

## 実装例
```csharp
try
{
  // 1. AnalyzeApplicationのインスタンス作成
  var csAnalyze = new AnalyzeApplication();

  // 2. 解析完了イベントの登録(C#ソースファイル単位で発行)
  csAnalyze.Register<IAnalyzed>(csAnalyze, (ev) =>
  {
      // C#ソースファイル名
      Console.WriteLine($"[{ev.FilePath}]");

      // C#ソースコードのコンソール出力
      Console.WriteLine(ev.FileRoot?.ToString());
  });

  var srcPath = "解析対象のフォルダ(相対パス可)";

  // 3. 解析開始
  csAnalyze.Analyze(srcPath);
}
catch (Exception ex)
{
  // 例外エラー発生時
  Console.WriteLine(ex.Message);
  return;
}
```

### FileRoot(IFileRoot)の詳細
```csharp
  /// <summary>
  /// ファイルルート インターフェース
  /// </summary>
  public interface IFileRoot
  {
    /// <summary>
    /// ファイルパス
    /// </summary>
    string FilePath { get; }

    /// <summary>
    /// 外部参照のクラス名とファイルパスのリスト
    /// </summary>
    Dictionary<string, string> OtherFiles { get; }

    /// <summary>
    /// 子メンバ
    /// </summary>
    List<IAnalyzeItem> Members { get; }
  }
```
※Membersの具体的なInterfaceは```IAnalyzeItem```を継承した```AnalyzeItems/IItemXxxxx```となる。  
　詳細は下記参照

### C#とInterfaceの対応表
```CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems```で定義  

|C#                   | 対応Interface                  | Membersに複数IAnalyzeItemあり |
|:--------------------|:-------------------------------|:----------------:|
|interface            | IItemInterface                 |        ○         |
|class                | IItemClass                     |        ○         |
|コンストラクタメソッド | IItemConstructor               |        ○         |
|フィールド            | IItemField                     |        ×         |
|プロパティ            | IItemProperty                  |        ○         |
|メソッド              | IItemMethod                    |        ○         |
|enum                 | IItemEnum                      |        ×         |
|foreach              | IItemForEach                   |        ○         |
|for                  | IItemFor                       |        ○         |
|while                | IItemWhile                     |        ○         |
|do-while             | IItemDo                        |        ○         |
|if                   | IItemIf                        |        ○         |
|else/else if         | IItemElseClause                |        ○         |
|switch               | IItemSwitch                    |        ○         |
|case                 | IItemSwitchCase                |        ○         |
|break                | IItemBreak                     |        ×         |
|continue             | ItemContinue                   |        ×         |
|return               | IItemReturn                    |        ×         |
|ローカルフィールド生成 | IItemStatementLocalDeclaration |        ×         |
|ローカルメソッド       | IItemLocalFunctio              |        ○         |
|式                   | IItemStatementExpression       |        ×         |




