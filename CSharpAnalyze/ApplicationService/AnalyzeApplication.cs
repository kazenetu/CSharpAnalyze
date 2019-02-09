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
    public void Analyze(string rootPath)
    {
      var fileRepository = new CSFileRepository();
      var analizeService = new AnalyzeService(rootPath, fileRepository);

      // 対象フォルダの解析を行う
      analizeService.Analyze();
    }

  }
}
