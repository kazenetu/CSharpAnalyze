using CSharpAnalyze.Domain.PublicInterfaces.Repository;
using CSharpAnalyze.Domain.Service;
using CSharpAnalyze.Infrastructure;

namespace CSharpAnalyze.ApplicationService
{
  /// <summary>
  /// C#解析アプリケーション
  /// </summary>
  /// <remarks>リクエストとレスポンスの変換アダプタなどを行う</remarks>
  public class AnalyzeApplication
  {
    /// <summary>
    /// C#解析処理
    /// </summary>
    /// <param name="rootPath">対象ルートフォルダ</param>
    public void Analyze(string rootPath)
    {
      // 対象フォルダの解析を行う
      Analyze(rootPath, new CSFileRepository());
    }

    /// <summary>
    /// C#解析処理
    /// </summary>
    /// <param name="rootPath">対象ルートフォルダ</param>
    /// <param name="fileRepository">ファイルリポジトリ</param>
    public void Analyze(string rootPath, ICSFileRepository fileRepository)
    {
      var analizeService = new AnalyzeService(rootPath, fileRepository);

      // 対象フォルダの解析を行う
      analizeService.Analyze();
    }

  }
}
