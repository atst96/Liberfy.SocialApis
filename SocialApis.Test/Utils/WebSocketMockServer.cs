using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace SocialApis.Test.Utils
{
    /// <summary>
    /// テスト用のWebSocketサーバ
    /// </summary>
    internal class WebSocketMockServer : IDisposable
    {
        private readonly string _url;
        private readonly string _subProtocol;
        private HttpListenerWebSocketContext _wssContext;
        private HttpListener _httpListener;

        /// <summary>
        /// <see cref="WebSocketMockServer"/>を生成する
        /// </summary>
        /// <param name="url"></param>
        /// <param name="subProtocol"></param>
        public WebSocketMockServer(string url, string subProtocol = null)
        {
            this._url = url;
            this._subProtocol = subProtocol;
        }

        /// <summary>
        /// WebSocketサーバを開始する
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            var httpListener = this._httpListener = new HttpListener();
            httpListener.Prefixes.Add(this._url);
            httpListener.Start();

            var httpContext = await httpListener.GetContextAsync().ConfigureAwait(false);
            if (!httpContext.Request.IsWebSocketRequest)
            {
                throw new NotSupportedException("WebSocket only.");
            }

            this._wssContext = await httpContext.AcceptWebSocketAsync(this._subProtocol).ConfigureAwait(false);
        }

        /// <summary>
        /// クライアントからのメッセージを受信する
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken? cancellationToken = null)
            => this._wssContext.WebSocket.ReceiveAsync(buffer, cancellationToken ?? CancellationToken.None);

        /// <summary>
        /// クライアントへメッセージを送信する
        /// </summary>
        /// <param name="data">送信するデータ</param>
        /// <param name="messageType">メッセージの種類</param>
        /// <param name="endOfMessage">メッセージの終端であるかのフラグ</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SendAsync(ArraySegment<byte> data, WebSocketMessageType messageType, bool endOfMessage = true, CancellationToken? cancellationToken = null)
            => this._wssContext.WebSocket.SendAsync(data, messageType, endOfMessage, cancellationToken ?? CancellationToken.None);

        /// <summary>
        /// 接続を閉じる
        /// </summary>
        /// <param name="status"></param>
        /// <param name="statusDescription"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Close(WebSocketCloseStatus status, string? statusDescription = null, CancellationToken? cancellationToken = null)
            => this._wssContext.WebSocket.CloseAsync(status, statusDescription, cancellationToken ?? CancellationToken.None);

        /// <summary>
        /// <see cref="IDisposable.Dispose"/>
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Close(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None).Wait();
            this._httpListener.Close();
        }
    }
}
