namespace ElementalAdventure.Common.Packets;

public class PacketRegistry {
    private readonly Dictionary<PacketType, object> _packetDeserializers = new();
    private readonly Dictionary<PacketType, IPacketHandler> _packetHandlers = new();

    public PacketRegistry() { }

    public void RegisterPacket<T>(PacketType type, Func<BinaryReader, T> deserializer, IPacketHandler handler) where T : IPacket {
        _packetDeserializers[type] = deserializer;
        _packetHandlers[type] = handler;
    }

    public IPacket DeserializePacket(PacketType type, BinaryReader reader) {
        return _packetDeserializers.TryGetValue(type, out object? raw) && raw is Func<BinaryReader, IPacket> deserializer
            ? deserializer(reader)
            : throw new InvalidOperationException($"No deserializer registered for packet type {type}");
    }

    public void TryHandlePacket(PacketConnection connection, IPacket packet) {
        if (_packetHandlers.TryGetValue(packet.Type, out IPacketHandler? handler))
            handler.Handle(connection, packet);
        else
            throw new InvalidOperationException($"No handler registered for packet type {packet.Type}");
    }

    public interface IPacketHandler {
        void Handle(PacketConnection connection, IPacket packet);
    }
}