using System.Net;
using System.Net.Sockets;

using ElementalAdventure.Common.Logging;
using ElementalAdventure.Common.Packets;

namespace ElementalAdventure.Common.Networking;

public class PacketClient {
    private readonly PacketRegistry _registry;
    private readonly IPEndPoint _endpoint;
    private PacketConnection? _connection;

    public event Action? OnConnected;
    public event Action<Exception?>? OnDisconnected;
    public event Action<IPacket>? OnPacketReceived;

    public PacketConnection? Connection => _connection;
    public Task? Awaiter { get; private set; }

    public PacketClient(PacketRegistry registry, IPEndPoint endpoint) {
        _registry = registry;
        _endpoint = endpoint;
        _connection = null;
    }

    public void Start(CancellationToken cancellationToken = default) {
        Awaiter = ConnectLoop(cancellationToken);
    }

    public async Task ConnectLoop(CancellationToken cancellationToken = default) {
        while (!cancellationToken.IsCancellationRequested) {
            Logger.Debug($"Attempting to connect to {_endpoint.Address}:{_endpoint.Port}...");
            TcpClient client = new();
            try {
                await client.ConnectAsync(_endpoint.Address, _endpoint.Port, cancellationToken);
                _connection = new PacketConnection(_registry, client);
                _connection.OnConnected += conn => OnConnected?.Invoke();
                _connection.OnDisconnected += (conn, ex) => {
                    _connection = null;
                    OnDisconnected?.Invoke(ex);
                };
                _connection.OnPacketReceived += (conn, packet) => OnPacketReceived?.Invoke(packet);
                await _connection.RunAsync(cancellationToken);
            } catch (OperationCanceledException) {
                break;
            } catch (Exception) {
                try {
                    await Task.Delay(1000, cancellationToken);
                } catch (OperationCanceledException) {
                    break;
                }
            } finally {
                client.Close();
                client.Dispose();
            }
        }
    }
}