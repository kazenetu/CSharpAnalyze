using CSharpAnalyze.Domain.PublicInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpAnalyze.Domain.Event
{
  /// <summary>
  /// イベントコンテナ
  /// </summary>
  public static class EventContainer
  {
    /// <summary>
    /// イベントハンドラリスト
    /// </summary>
    private static List<(object instance, Delegate callback)> Handles = new List<(object instance, Delegate callback)>();

    /// <summary>
    /// 排他ロック用オブジェクト
    /// </summary>
    private static readonly object lockObject =  new object ();

    /// <summary>
    /// イベントの登録
    /// </summary>
    /// <param name="instance">登録対象のインスタンス</param>
    /// <param name="callback">イベントハンドラ</param>
    public static void Register<T>(object instance, Action<T> callback) where T : IEvent
    {
      lock (lockObject)
      {
        Handles.Add((instance, callback));
      }
    }

    /// <summary>
    /// イベントの削除
    /// </summary>
    /// <param name="instance">登録対象のインスタンス</param>
    /// <param name="callback">イベントハンドラ</param>
    public static void Unregister<T>(object instance) where T : IEvent
    {
      lock (lockObject)
      {
        var targets = Handles.Where(handle => handle.instance == instance && handle.callback is Action<T>).ToList();
        targets.ForEach(item => Handles.Remove((item.instance, item.callback)));
      }
    }

    /// <summary>
    /// イベントの全削除
    /// </summary>
    public static void UnregisterAll()
    {
      lock (lockObject)
      {
        Handles.Clear();
      }
    }

    /// <summary>
    /// イベント発行
    /// </summary>
    /// <param name="args">発行イベント</param>
    public static void Raise<T>(T args)
    {
      lock (lockObject)
      {
        var targets = Handles.Where(handle => handle.callback is Action<T>).ToList();
        foreach (var target in targets)
        {
          ((Action<T>)target.callback)(args);
        }
      }
    }
  }
}
