﻿using System.Collections.Generic;

namespace CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems
{
  /// <summary>
  /// インターフェース:コンストラクタ
  /// </summary>
  public interface IItemConstructor : IAnalyzeItem
  {
    /// <summary>
    /// パラメーターリスト
    /// </summary>
    List<(string name, List<IExpression> expressions)> Args { get; }
  }
}