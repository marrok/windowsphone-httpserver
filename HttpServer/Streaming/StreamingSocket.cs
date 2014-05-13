using HttpServer.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace HttpServer.Streaming
{
    public delegate Task StreamingSocketRunDelegate(DataWriter writer);

    public delegate Task StreamingSocketStopDelegate();

    internal class StreamingSocket
    {
        private DatagramSocket _socket;
        private DatagramSocket _pingSocket;

        private volatile bool _inUse = false;
        private volatile bool _stopStreaming = false;

        private static readonly int PING_SLEEP_TIME = 1000;
        private int _pingsWithoutResponse = 0;

        public void Stop()
        {
            _stopStreaming = true;
        }

        public void Start(String destinationIp, String destinationPort, String pingPort, StreamingSocketRunDelegate runDelegate, StreamingSocketStopDelegate stopDelegate)
        {
            if (_inUse)
            {
                throw new StreamingException("This Streaming Socket is already in use");
            }

            _stopStreaming = false;

            Task.Run(async () =>
            {
                _socket = new DatagramSocket();
                await _socket.ConnectAsync(new Windows.Networking.HostName(destinationIp), destinationPort);
                using (var writer = new DataWriter(_socket.OutputStream))
                {
                    await runDelegate(writer);

                    while (!_stopStreaming)
                    {
                        Thread.Sleep(1000);
                    }
                }
                await stopDelegate();
                _socket.Dispose();
                _inUse = false;
            });

            Task.Run(async () =>
            {
                _pingSocket = new DatagramSocket();
                _pingSocket.MessageReceived += PongMessageReceived;
                await _pingSocket.ConnectAsync(new Windows.Networking.HostName(destinationIp), pingPort);

                using (var writer = new DataWriter(_pingSocket.OutputStream))
                {
                    while (!_stopStreaming)
                    {
                        writer.WriteString("PING");
                        await writer.StoreAsync();
                        Thread.Sleep(PING_SLEEP_TIME);

                        if (++_pingsWithoutResponse > 5)
                        {
                            _stopStreaming = true;
                        }
                    }
                }
                _pingSocket.Dispose();
            });
        }

        private void PongMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            _pingsWithoutResponse = 0;
        }
    }
}
