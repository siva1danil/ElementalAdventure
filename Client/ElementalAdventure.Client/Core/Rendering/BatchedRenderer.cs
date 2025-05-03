using System.Runtime.CompilerServices;

using ElementalAdventure.Client.Core.Assets;

namespace ElementalAdventure.Client.Core.Rendering;

public class BatchedRenderer<T> : IRenderer where T : notnull {
    private readonly AssetManager<T> _assetManager;
    private readonly Dictionary<BatchKey, BatchData> _batches;

    public BatchedRenderer(AssetManager<T> assetManager) {
        _assetManager = assetManager;
        _batches = [];
    }

    public Span<byte> AllocateInstance(object ownerIdentity, int ownerIndex, string shaderProgram, string textureAtlas, Span<byte> vertexData, int instanceSize) {
        BatchKey key = new(shaderProgram, textureAtlas, FastHash(vertexData));
        long owner = (long)RuntimeHelpers.GetHashCode(ownerIdentity) << 32 | (long)ownerIndex;

        if (!_batches.ContainsKey(key))
            _batches[key] = new BatchData(vertexData.ToArray(), [], []);

        _batches[key].CurrentSlots.Add(owner);
        if (_batches[key].Slots.ContainsKey(owner)) {
            if (_batches[key].Slots[owner].Size == instanceSize) {
                // Return existing slot
                return new Span<byte>(_batches[key].InstanceData, _batches[key].Slots[owner].Offset, instanceSize);
            } else {
                // Deallocate old slot
                int oldOffset = _batches[key].Slots[owner].Offset, oldSize = _batches[key].Slots[owner].Size;
                Array.Copy(_batches[key].InstanceData, oldOffset + oldSize, _batches[key].InstanceData, oldOffset, _batches[key].InstanceData.Length - (oldOffset + oldSize));

                // Allocate new slot
                Array.Resize(ref _batches[key].InstanceData, _batches[key].InstanceData.Length - oldSize + instanceSize);
                _batches[key].Slots[owner] = new BatchSlot { Offset = _batches[key].InstanceData.Length - instanceSize, Size = instanceSize };

                // Reorder offsets
                foreach (KeyValuePair<long, BatchSlot> slot in _batches[key].Slots)
                    if (slot.Value.Offset >= oldOffset)
                        _batches[key].Slots[slot.Key] = slot.Value with { Offset = slot.Value.Offset - oldSize };
                _batches[key].Slots[owner] = new BatchSlot { Offset = _batches[key].InstanceData.Length - instanceSize, Size = instanceSize };
                return new Span<byte>(_batches[key].InstanceData, _batches[key].Slots[owner].Offset, instanceSize);
            }
        } else {
            // Allocate new slot
            Array.Resize(ref _batches[key].InstanceData, _batches[key].InstanceData.Length + instanceSize);
            _batches[key].Slots[owner] = new BatchSlot { Offset = _batches[key].InstanceData.Length - instanceSize, Size = instanceSize };
            return new Span<byte>(_batches[key].InstanceData, _batches[key].Slots[owner].Offset, instanceSize);
        }
    }

    public void Commit() {
        foreach (KeyValuePair<BatchKey, BatchData> batch in _batches) {
            // Invalidate inactive
            if (batch.Value.PreviousSlots.Count != batch.Value.CurrentSlots.Count) {
                int size = batch.Value.InstanceData.Length;
                foreach (long owner in batch.Value.PreviousSlots) {
                    if (!batch.Value.CurrentSlots.Contains(owner)) {
                        // Deallocate old slot
                        int oldOffset = batch.Value.Slots[owner].Offset, oldSize = batch.Value.Slots[owner].Size;
                        Array.Copy(batch.Value.InstanceData, oldOffset + oldSize, batch.Value.InstanceData, oldOffset, batch.Value.InstanceData.Length - (oldOffset + oldSize));
                        size -= oldSize;

                        // Reorder offsets
                        foreach (KeyValuePair<long, BatchSlot> slot in batch.Value.Slots)
                            if (slot.Value.Offset >= oldOffset)
                                batch.Value.Slots[slot.Key] = slot.Value with { Offset = slot.Value.Offset - oldSize };
                        batch.Value.Slots.Remove(owner);
                    }
                }
                if (size != batch.Value.InstanceData.Length)
                    Array.Resize(ref batch.Value.InstanceData, size);
            }

            // Swap maps
            (batch.Value.PreviousSlots, batch.Value.CurrentSlots) = (batch.Value.CurrentSlots, batch.Value.PreviousSlots);
            batch.Value.CurrentSlots.Clear();
        }
    }

    public void Render() {
        // TODO: OpenGL rendering
    }

    private static int FastHash(Span<byte> span) {
        int hash = 17;
        foreach (byte b in span)
            hash = hash * 31 + b;
        return hash;
    }

    private readonly record struct BatchKey(string ShaderProgram, string TextureAtlas, int VertexDataHash);
    private class BatchData(byte[] vertexData, byte[] instanceData, Dictionary<long, BatchSlot> slots) {
        public byte[] VertexData = vertexData;
        public byte[] InstanceData = instanceData;
        public Dictionary<long, BatchSlot> Slots = slots;
        public HashSet<long> PreviousSlots = [.. slots.Keys], CurrentSlots = [.. slots.Keys];
    }
    private struct BatchSlot {
        public int Offset;
        public int Size;
    }
}