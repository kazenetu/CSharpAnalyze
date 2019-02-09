using CSharpAnalyze.Domain.Model.Analyze;
using CSharpAnalyze.Domain.Model.File;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSharpAnalyze.Domain.Service
{
  /// <summary>
  /// C#解析サービス
  /// </summary>
  internal class AnalyzeService
  {
    /// <summary>
    /// ファイルアクセスリポジトリ
    /// </summary>
    private ICSFileRepository FileRepository;

    /// <summary>
    /// 解析対象のC#ファイルルートパス
    /// </summary>
    private string RootPath;

    #region コンストラクタ
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="rootPath">ルートパス</param>
    /// <param name="fileRepository">C#ファイルデータ読み込み</param>
    public AnalyzeService(string rootPath, ICSFileRepository fileRepository)
    {
      RootPath = rootPath;
      FileRepository = fileRepository;
    }
    #endregion

    #region パブリックメソッド
    /// <summary>
    /// 解析を実施する
    /// </summary>
    public void Analyze()
    {
      // セマンティックモデルのリストを作成する
      var models = CreateModels();
      if (!models.Any())
      {
        // リスト作成失敗
        throw new Exception("ファイルの解析に失敗しました。");
      }

      // 解析処理収集
      foreach (var model in models)
      {
        FileRoot.Create(model);
      }
    }
    #endregion

    #region プライベートメソッド

    /// <summary>
    /// セマンティックモデルを作成する
    /// </summary>
    /// <returns>作成結果</returns>
    private List<SemanticModel> CreateModels()
    {
      // C#ファイル情報リストを取得
      var csFiles = CSFile.GetCSFileList(RootPath, FileRepository);

      // 編集クラスを作成
      var mscorlib = MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location);
      var treeList = csFiles.Select(info => CSharpSyntaxTree.ParseText(info.SourceCode, null, info.RelativePath)).ToList();
      var compilation = CSharpCompilation.Create("", treeList, new[] { mscorlib });

      // セマンティックモデルのリストを取得する
      var models = new List<SemanticModel>();
      foreach (var model in treeList)
      {
        models.Add(compilation.GetSemanticModel(model));
      }
      return models;
    }

    #endregion
  }
}
