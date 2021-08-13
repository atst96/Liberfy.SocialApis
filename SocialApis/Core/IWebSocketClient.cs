﻿using System;
using System.Threading.Tasks;

namespace SocialApis.Core
{
    /// <summary>
    /// WebSocketラッパークラスのインタフェース
    /// </summary>
    public interface IWebSocketClient : IDisposable
    {
        /// <summary>
        /// WebSocket通信が接続中かどうか取得する。
        /// </summary>
        bool IsConnecting { get; }

        /// <summary>
        /// 接続を開始する。
        /// </summary>
        /// <returns></returns>
        Task Connect();

        /// <summary>
        /// 接続を閉じる。
        /// </summary>
        /// <returns></returns>
        Task Close();
    }
}
