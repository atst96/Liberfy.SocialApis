using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace SocialApis.Core
{
    /// <summary>
    /// WebSocketクライアント
    /// </summary>
    internal abstract class WebSocketClient : IWebSocketClient, IDisposable
    {
        /// <summary>
        /// データ受信時のバッファサイズ
        /// </summary>
        private const int BufferSize = 2048;

        private CancellationToken _cancellationToken;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// WebSocketクライアント
        /// </summary>
        protected ClientWebSocket Connection { get; }


        /// <summary>
        /// 接続中かどうか取得する。
        /// </summary>
        public bool IsConnecting { get; private set; } = false;

        /// <summary>
        /// <see cref="WebSocketClient"/>を生成する。
        /// </summary>
        protected WebSocketClient()
        {
            this.Connection = WebFactory.CreateWebSocketClient();
        }

        /// <summary>
        /// 接続を開始する。
        /// </summary>
        /// <returns></returns>
        public Task Connect() => this.Connect(CancellationToken.None);

        /// <summary>
        /// 接続を開始する。
        /// </summary>
        public async Task Connect(CancellationToken cancellationToken)
        {
            var sem = this._semaphore;
            await sem.WaitAsync().ConfigureAwait(false);

            if (this.IsConnecting)
            {
                throw new InvalidOperationException();
            }

            this.IsConnecting = true;
            this._cancellationToken = cancellationToken;

            sem.Release();

            var result = await this.TryConnect(cancellationToken).ConfigureAwait(false);
            if (!result)
            {
                return;
            }

            this.StartPooling(cancellationToken);
        }

        /// <summary>
        /// 接続先を取得する
        /// </summary>
        /// <returns></returns>
        protected abstract Uri GetConnectUri();

        /// <summary>
        /// 接続を開始する
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected async Task<bool> TryConnect(CancellationToken token)
        {
            var wss = this.Connection;

            try
            {
                await wss.ConnectAsync(this.GetConnectUri(), token).ConfigureAwait(false);
            }
            catch (TaskCanceledException tcex)
            {
                // TODO: LOG
                return false;
            }

            return true;
        }

        /// <summary>
        /// 受信処理を開始する
        /// </summary>
        /// <returns></returns>
        private void StartPooling(CancellationToken cancellationToken) => Task.Run(async () =>
        {
            var buffer = new byte[BufferSize].AsMemory();
            var wss = this.Connection;

            try
            {
                while (this.Connection.State == WebSocketState.Open)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var dataCollection = new LinkedList<byte[]>();

                    int dataLength = 0;
                    bool endOfMessage = false;
                    WebSocketMessageType messageType;
                    do
                    {
                        var result = await wss.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await this.Close().ConfigureAwait(false);
                            return;
                        }

                        int dataCount = result.Count;
                        dataCollection.AddLast(buffer[0..dataCount].ToArray());
                        dataLength += dataCount;

                        messageType = result.MessageType;
                        endOfMessage = result.EndOfMessage;
                    }
                    while (!endOfMessage);

                    cancellationToken.ThrowIfCancellationRequested();

                    var dest = ConcatBytes(dataCollection, dataLength);

                    switch (messageType)
                    {
                        case WebSocketMessageType.Text:
                            this.OnReceiveText(dest);
                            break;
                        case WebSocketMessageType.Binary:
                            this.OnReceiveBinary(dest);
                            break;
                    }
                }
            }
            catch (TaskCanceledException tcex)
            {
                // TODO: LOG
                await this.Close().ConfigureAwait(false);
            }
        });

        /// <summary>
        /// 接続を終了する。
        /// </summary>
        /// <returns></returns>
        public async Task Close()
        {
            var connection = this.Connection;
            var waltHandler = this._semaphore;

            await waltHandler.WaitAsync().ConfigureAwait(false);
            if (!this.IsConnecting)
            {
                return;
            }

            await connection.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None)
                .ConfigureAwait(false);

            this.IsConnecting = false;

            waltHandler.Release();

            this.OnClosed();
        }

        /// <summary>
        /// テキストデータ受取時
        /// </summary>
        /// <param name="data">受信したデータ</param>
        protected virtual void OnReceiveText(byte[] data)
        {
        }

        /// <summary>
        /// バイナリデータ受取時
        /// </summary>
        /// <param name="data">受信したデータ</param>
        protected virtual void OnReceiveBinary(byte[] data)
        {
        }

        /// <summary>
        /// 接続終了時
        /// </summary>
        protected virtual void OnClosed()
        {
        }

        /// <summary>
        /// 複数のバイト配列を結合する。
        /// </summary>
        /// <param name="dataCollection">バイト配列</param>
        /// <param name="dataCount">コピー先配列のバイト数</param>
        /// <returns></returns>
        private static byte[] ConcatBytes(LinkedList<byte[]> dataCollection, int dataCount)
        {
            byte[] dest = new byte[dataCount];
            int offset = 0;

            foreach (var src in dataCollection)
            {
                int length = src.Length;

                Buffer.BlockCopy(src, 0, dest, offset, length);

                offset += length;
            }

            return dest;
        }

        /// <summary>
        /// インスタンスを破棄する。
        /// </summary>
        protected virtual void Dispose()
        {
            this.Connection.Dispose();
            this._semaphore.Dispose();
        }

        /// <summary>
        /// インスタンスを破棄する。
        /// </summary>
        void IDisposable.Dispose() => this.Dispose();
    }
}
