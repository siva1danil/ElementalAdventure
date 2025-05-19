using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using ElementalAdventure.Common.Logging;
using ElementalAdventure.Common.Packets;

namespace ElementalAdventure.Common;

public class PacketConnection {
    private readonly PacketRegistry _registry;
    private readonly TcpClient _client;

    public event Action<PacketConnection>? OnConnected;
    public event Action<PacketConnection, Exception?>? OnDisconnected;
    public event Action<PacketConnection, IPacket>? OnPacketReceived;

    public PacketConnection(PacketRegistry registry, TcpClient client) {
        _registry = registry;
        _client = client;
    }

    public async Task RunAsync(CancellationToken cancellationToken) {
        using NetworkStream stream = _client.GetStream();
        using BinaryReader reader = new(stream);
        Exception? reason = null;

        Logger.Debug($"Establishing connection to {_client.Client.RemoteEndPoint}");
        OnConnected?.Invoke(this);
        while (_client.Connected && !cancellationToken.IsCancellationRequested) {
            try {
                byte[] len = new byte[sizeof(ushort)];
                await stream.ReadExactlyAsync(len, 0, len.Length, cancellationToken);
                byte[] type = new byte[sizeof(ushort)];
                await stream.ReadExactlyAsync(type, 0, type.Length, cancellationToken);
                byte[] buffer = new byte[((len[0] << 8) | len[1]) - sizeof(ushort) * 2];
                await stream.ReadExactlyAsync(buffer, 0, buffer.Length, cancellationToken);

                using MemoryStream ms = new(buffer);
                using BinaryReader packetReader = new(ms);
                IPacket packet = _registry.DeserializePacket((PacketType)((type[0] << 8) | type[1]), packetReader);
                OnPacketReceived?.Invoke(this, packet);
            } catch (EndOfStreamException) {
                break;
            } catch (OperationCanceledException) {
                break;
            } catch (Exception ex) {
                Logger.Error($"Error in connection {_client.Client.RemoteEndPoint}: {ex.Message}");
                reason = ex;
                break;
            }
        }
        Logger.Debug($"Closed connection to {_client.Client.RemoteEndPoint}.");

        _client.Close();
        OnDisconnected?.Invoke(this, reason);
    }

    public void Send(IPacket packet) {
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);

        packet.Serialize(writer);
        writer.Flush();

        ushort len = (ushort)(stream.Length + sizeof(ushort) * 2);
        ushort type = (ushort)packet.Type;

        _client.GetStream().Write([(byte)(len >> 8), (byte)(len & 0xFF), (byte)(type >> 8), (byte)(type & 0xFF)], 0, sizeof(ushort) * 2);
        _client.GetStream().Write(stream.ToArray(), 0, (int)stream.Length);
        _client.GetStream().Flush();
    }
}