using System.Net;
using System.Net.Sockets;

using ElementalAdventure.Common.Logging;
using ElementalAdventure.Common.Packets;

namespace ElementalAdventure.Common.Networking;

public class PacketClient {
    private readonly PacketRegistry _registry;
    private readonly IPEndPoint _endpoint;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public event Action? OnConnected;
    public event Action<Exception?>? OnDisconnected;
    public event Action<IPacket>? OnPacketReceived;

    public PacketConnection? Connection { get; private set; }
    public Task? Awaiter { get; private set; }

    public PacketClient(PacketRegistry registry, IPEndPoint endpoint) {
        _cancellationTokenSource = new CancellationTokenSource();
        _registry = registry;
        _endpoint = endpoint;
        Connection = null;
    }

    public void Start() {
        if (Awaiter == null)
            Awaiter = ConnectLoop();
        else
            Logger.Warn("Start called while already running.");
    }

    public void Stop() {
        _cancellationTokenSource.Cancel();
        Awaiter?.Wait();
    }

    private async Task ConnectLoop() {
        while (!_cancellationTokenSource.Token.IsCancellationRequested) {
            Logger.Debug($"Attempting to connect to {_endpoint.Address}:{_endpoint.Port}...");
            TcpClient client = new();
            try {
                await client.ConnectAsync(_endpoint.Address, _endpoint.Port, _cancellationTokenSource.Token);
                Connection = new PacketConnection(_registry, client);
                Connection.OnConnected += conn => OnConnected?.Invoke();
                Connection.OnDisconnected += (conn, ex) => {
                    Connection = null;
                    OnDisconnected?.Invoke(ex);
                };
                Connection.OnPacketReceived += (conn, packet) => OnPacketReceived?.Invoke(packet);
                await Connection.RunAsync(_cancellationTokenSource.Token);
            } catch (OperationCanceledException) {
                break;
            } catch (Exception) {
                try {
                    await Task.Delay(1000, _cancellationTokenSource.Token);
                } catch (OperationCanceledException) {
                    break;
                }
            } finally {
                client.Close();
                client.Dispose();
                Connection = null;
            }
        }
    }
}