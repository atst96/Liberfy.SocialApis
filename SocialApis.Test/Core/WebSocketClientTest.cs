using SocialApis.Core;
using SocialApis.Test.Utils;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SocialApis.Test.Core
{
    /// <summary>
    /// <see cref="WebSocketClient"/>のテスト
    /// </summary>
    public class WebSocketClientTest
    {
        /// <summary>
        /// テスト用のWebSocketクライアント
        /// </summary>
        internal class TestWebSocket : WebSocketClient
        {
            private string _url;

            private AutoResetEvent _waitHandler { get; } = new(false);

            private byte[] _receivedText;

            private byte[] _receivedBinary;

            public TestWebSocket(string url)
            {
                this._url = url;
            }

            protected override Uri GetConnectUri() => new(this._url);

            protected override void OnReceiveBinary(byte[] data)
            {
                base.OnReceiveBinary(data);

                this._receivedBinary = data;
                this._waitHandler.Set();
            }

            protected override void OnReceiveText(byte[] data)
            {
                base.OnReceiveText(data);

                this._receivedText = data;
                this._waitHandler.Set();
            }

            public byte[] ReceiveText()
            {
                this.Wait();

                return this._receivedText;
            }

            public byte[] ReceiveBinary()
            {
                this.Wait();

                return this._receivedBinary;
            }

            protected override void OnClosed()
            {
                base.OnClosed();

                this._waitHandler.Set();
            }

            private void Wait() => this._waitHandler.WaitOne();
        }

        /// <summary>
        /// 文字列をバイナリデータに変換
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static byte[] GetBytes(string value) => Encoding.UTF8.GetBytes(value);

        /// <summary>
        /// バイナリデータを文字列に変換
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string GetString(byte[] bytes) => Encoding.UTF8.GetString(bytes);

        /// <summary>
        /// <seealso cref="ReceiveTest"/>のパラメータを生成する
        /// </summary>
        /// <returns></returns>
        public static object[][] GenerateReceiveTestParameters() => new object[][]
        {
            // 空文字
            new[] { new[] { "" } },
            // 1件送信
            new[] { new[] { "test" } },
            new[] { new[] { new string('a', 2048) }},
            new[] { new[] { new string('b', 2049) }},
            new[] { new[] { new string('c', 5120) }},
            // 2件以上送信
            new[] { new[] { "test1", "test2" }},
            new[] { new[] { new string('d', 2000), new string('e', 321) }},
        };

        /// <summary>
        /// <see cref="WebSocketClient.OnReceiveText(byte[])"/>のテスト
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [Theory]
        [MemberData(nameof(GenerateReceiveTestParameters))]
        public async Task ReceiveTextTest(string[] values)
        {
            // 期待値
            var expected = string.Join("", values);
            byte[][] sendData = values.Select(GetBytes).ToArray();

            var client = new TestWebSocket("ws://localhost:8080/");
            using (var server = new WebSocketMockServer("http://localhost:8080/"))
            {
                // サーバー起動後にクライアント側にデータ送信
                var serverTask = server.Start()
                    .ContinueWith(async t =>
                    {
                        for (int i = 0, m = sendData.Length - 1; i <= m; ++i)
                        {
                            // 最後の要素のみendOfMessageにTrueをセットする
                            await server.SendAsync(sendData[i], WebSocketMessageType.Text, i == m)
                                .ConfigureAwait(false);
                        }
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);

                // クライアントを接続
                await client.Connect(CancellationToken.None).ConfigureAwait(false);

                // 期待値通りに取得できていることを確認
                var actual = GetString(client.ReceiveText());
                Assert.Equal(expected, actual);

                // サーバー側スレッドの終了待機
                await serverTask.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// <see cref="WebSocketClient.OnReceiveBinary(byte[])"/>のテスト
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [Theory]
        [MemberData(nameof(GenerateReceiveTestParameters))]
        public async Task ReceiveBinaryTest(string[] values)
        {
            // 期待値
            var expected = GetBytes(string.Join("", values));
            byte[][] sendData = values.Select(GetBytes).ToArray();

            var client = new TestWebSocket("ws://localhost:8080/");
            using (var server = new WebSocketMockServer("http://localhost:8080/"))
            {
                // サーバー起動後にクライアント側にデータ送信
                var serverTask = server.Start()
                    .ContinueWith(async t =>
                    {
                        for (int i = 0, m = sendData.Length - 1; i <= m; ++i)
                        {
                            // 最後の要素のみendOfMessageにTrueをセットする
                            await server.SendAsync(sendData[i], WebSocketMessageType.Text, i == m)
                                .ConfigureAwait(false);
                        }
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);

                // クライアントを接続
                await client.Connect(CancellationToken.None).ConfigureAwait(false);

                // 期待値通りに取得できていることを確認
                var actual = client.ReceiveText();
                Assert.Equal(expected, actual);

                // サーバー側スレッドの終了待機
                await serverTask.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// <see cref="WebSocketClient.Close"/>のテスト
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CloseTest()
        {
            var client = new TestWebSocket("ws://localhost:8081/");
            using (var server = new WebSocketMockServer("http://localhost:8081/"))
            {
                // サーバーを起動
                var serverTask = server.Start();

                // クライアントをサーバーに接続
                var clientTask = client.Connect(CancellationToken.None)
                    .ContinueWith(
                        t => client.Close().ConfigureAwait(false),
                        TaskContinuationOptions.OnlyOnRanToCompletion);

                // 接続完了を待機
                await serverTask.ConfigureAwait(false);

                byte[] buffer = new byte[1024];
                var response = await server.ReceiveAsync(buffer).ConfigureAwait(false);

                // サーバ側でCloseメッセージが受信されていることを確認
                Assert.Equal(WebSocketMessageType.Close, response.MessageType);

                // クライアントのスレッド終了待機
                await clientTask.ConfigureAwait(false);
            }
        }
    }
}
