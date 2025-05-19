using System.Net;
using System.Net.Sockets;

using ElementalAdventure.Common.Packets;

namespace ElementalAdventure.Common;

public class PacketServer {
    private readonly PacketRegistry _registry;
    private readonly TcpListener _listener;
    private readonly List<PacketConnection> _connections;

    public event Action<PacketConnection>? OnClientConnected;
    public event Action<PacketConnection, Exception?>? OnClientDisconnected;
    public event Action<PacketConnection, IPacket>? OnPacketReceived;

    public Task? Awaiter { get; private set; }

    public PacketServer(PacketRegistry registry, IPEndPoint endpoint) {
        _registry = registry;
        _listener = new TcpListener(endpoint);
        _connections = [];
    }

    public void Start(CancellationToken cancellationToken = default) {
        try {
            _listener.Start();
            Awaiter = AcceptLoop(cancellationToken);
        } catch (SocketException ex) {
            throw new InvalidOperationException($"Failed to start server on {((IPEndPoint)_listener.LocalEndpoint).Address}:{((IPEndPoint)_listener.LocalEndpoint).Port}. Is the port already in use? ({ex.Message})", ex);
        }
    }

    private async Task AcceptLoop(CancellationToken cancellationToken) {
        while (!cancellationToken.IsCancellationRequested) {
            TcpClient client = await _listener.AcceptTcpClientAsync(cancellationToken);

            PacketConnection connection = new PacketConnection(_registry, client);
            connection.OnConnected += conn => OnClientConnected?.Invoke(conn);
            connection.OnDisconnected += (conn, ex) => {
                _connections.Remove(conn);
                OnClientDisconnected?.Invoke(conn, ex);
            };
            connection.OnPacketReceived += (conn, packet) => OnPacketReceived?.Invoke(conn, packet);

            _connections.Add(connection);
            _ = connection.RunAsync(cancellationToken);
        }

        _listener.Stop();
        Awaiter = null;
    }
}