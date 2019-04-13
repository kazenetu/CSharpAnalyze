using CSharpAnalyze.Domain.Event;
using CSharpAnalyze.Domain.PublicInterfaces;
using CSharpAnalyze.Domain.PublicInterfaces.Repository;
using CSharpAnalyze.Domain.Service;
using CSharpAnalyze.Infrastructure;
using System;

namespace CSharpAnalyze.ApplicationService
{
  /// <summary>
  /// C#解析アプリケーション
  /// </summary>
  /// <remarks>リクエストとレスポンスの変換アダプタなどを行う</remarks>
  public class AnalyzeApplication
  {
    /// <summary>
    /// イベントコンテナ
    /// </summary>
    private EventContainer eventContainer = new EventContainer();

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
      analizeService.Analyze(eventContainer);
    }

    /// <summary>
    /// イベントの登録
    /// </summary>
    /// <param name="instance">登録対象のインスタンス</param>
    /// <param name="callback">イベントハンドラ</param>
    public void Register<T>(object instance, Action<T> callback) where T : IEvent
    {
      eventContainer.Register<T>(instance, callback);
    }

    /// <summary>
    /// イベントの削除
    /// </summary>
    /// <param name="instance">登録対象のインスタンス</param>
    public void Unregister<T>(object instance) where T : IEvent
    {
      eventContainer.Unregister<T>(instance);
    }
  }
}
