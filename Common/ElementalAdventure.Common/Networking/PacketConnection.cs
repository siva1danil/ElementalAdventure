using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using ElementalAdventure.Common.Logging;
using ElementalAdventure.Common.Packets;

namespace ElementalAdventure.Common;

public class PacketConnection {
    private readonly PacketRegistry _registry;
    private readonly TcpClient _client;

    public Dictionary<string, object> SessionStorage { get; }

    public event Action<PacketConnection>? OnConnected;
    public event Action<PacketConnection, Exception?>? OnDisconnected;
    public event Action<PacketConnection, IPacket>? OnPacketReceived;

    public PacketConnection(PacketRegistry registry, TcpClient client) {
        _registry = registry;
        _client = client;
        SessionStorage = [];
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
            } catch (OperationCanceledException) {
                break;
            } catch (EndOfStreamException) {
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

    public async Task SendAsync(IPacket packet, CancellationToken cancellationToken = default) {
        try {
            using MemoryStream stream = new();
            using BinaryWriter writer = new(stream);

            packet.Serialize(writer);
            writer.Flush();

            ushort len = (ushort)(stream.Length + sizeof(ushort) * 2);
            ushort type = (ushort)packet.Type;

            byte[] header = [(byte)(len >> 8), (byte)(len & 0xFF), (byte)(type >> 8), (byte)(type & 0xFF)];
            NetworkStream netStream = _client.GetStream();

            await netStream.WriteAsync(header, 0, header.Length, cancellationToken).ConfigureAwait(false);
            await netStream.WriteAsync(stream.GetBuffer(), 0, (int)stream.Length, cancellationToken).ConfigureAwait(false);
            await netStream.FlushAsync(cancellationToken).ConfigureAwait(false);

            Logger.Debug($"Packet sent: {packet.GetType().Name} ({packet.Type})");
        } catch (Exception ex) {
            Logger.Error($"Error sending packet: {ex.Message}");
            try {
                _client.Close();
            } catch { }
        }
    }
}