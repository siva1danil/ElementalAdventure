using System.Diagnostics;
using System.Runtime.CompilerServices;

using ElementalAdventure.Client.Core.Resources.Data;
using ElementalAdventure.Client.Core.Resources.HighLevel;
using ElementalAdventure.Client.Core.Resources.OpenGL;
using ElementalAdventure.Common.Assets;
using ElementalAdventure.Common.Logging;

using OpenTK.Graphics.OpenGL4;

namespace ElementalAdventure.Client.Core.Rendering;

public class BatchedRenderer : IRenderer {
    private readonly AssetManager _assetManager;
    private readonly IUniformProvider _uniformProvider;
    private readonly Dictionary<BatchKey, BatchData> _batches;

    public BatchedRenderer(AssetManager assetManager, IUniformProvider uniformProvider) {
        _assetManager = assetManager;
        _uniformProvider = uniformProvider;
        _batches = [];
    }

    public Span<byte> AllocateInstance(object ownerIdentity, int ownerIndex, AssetID shaderProgram, AssetID textureAtlas, Span<byte> vertexData, int instanceSize) {
        BatchKey key = new(shaderProgram, textureAtlas, FastHash(vertexData));
        long owner = (long)RuntimeHelpers.GetHashCode(ownerIdentity) << 32 | (long)ownerIndex;

        if (!_batches.ContainsKey(key))
            _batches[key] = new BatchData(_assetManager.Get<ShaderProgram>(shaderProgram).Layout, vertexData.ToArray(), [], new byte[_assetManager.Get<ShaderProgram>(shaderProgram).Layout.UniformDataSize], []);

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
        foreach (KeyValuePair<BatchKey, BatchData> batch in _batches) {
            // Upload VertexArray
            _uniformProvider.GetUniformData(batch.Key.ShaderProgram, batch.Key.TextureAtlas, batch.Value.UniformData);
            batch.Value.VertexArrayInstanced.SetGlobalData(batch.Value.VertexData);
            batch.Value.VertexArrayInstanced.SetInstanceData(batch.Value.InstanceData);
            batch.Value.UniformBuffer.SetData(batch.Value.UniformData);

            // Use ShaderProgram
            GL.UseProgram(_assetManager.Get<ShaderProgram>(batch.Key.ShaderProgram).Id);
            // Use TextureAtlas
            GL.ActiveTexture(TextureUnit.Texture0);
            if (batch.Key.TextureAtlas == AssetID.None) GL.BindTexture(TextureTarget.Texture2D, 0);
            else if (_assetManager.TryGet(batch.Key.TextureAtlas, out TextureAtlas? atlas)) GL.BindTexture(TextureTarget.Texture2D, atlas!.Id);
            else if (_assetManager.TryGet(batch.Key.TextureAtlas, out MsdfFont? msdfFont)) GL.BindTexture(TextureTarget.Texture2D, msdfFont!.Id);
            else throw new ArgumentException($"BatchKey {batch.Key} has an invalid TextureAtlas.");
            // Use UniformBufferObject
            GL.BindBuffer(BufferTarget.UniformBuffer, batch.Value.UniformBuffer.Id);
            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, batch.Value.UniformBuffer.Id);
            // Use VertexArray
            GL.BindVertexArray(batch.Value.VertexArrayInstanced.Id);

            // Draw instances
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, batch.Value.VertexData.Length / batch.Value.VertexArrayInstanced.VertexDataSize, batch.Value.InstanceData.Length / batch.Value.VertexArrayInstanced.InstanceDataSize);
        }
    }

    public void Dispose() {
        foreach (KeyValuePair<BatchKey, BatchData> batch in _batches) {
            Logger.Debug($"Disposing batch BatchKey{{{batch.Key.ShaderProgram},{batch.Key.TextureAtlas},{batch.Key.VertexDataHash}}} of {batch.Value.VertexData.Length}+{batch.Value.InstanceData.Length} bytes");
            batch.Value.VertexArrayInstanced.Dispose();
            batch.Value.UniformBuffer.Dispose();
        }
        _batches.Clear();
        GC.SuppressFinalize(this);
    }

    private static int FastHash(Span<byte> span) {
        int hash = 17;
        foreach (byte b in span)
            hash = hash * 31 + b;
        return hash;
    }

    private readonly record struct BatchKey(AssetID ShaderProgram, AssetID TextureAtlas, int VertexDataHash);
    private class BatchData(DataLayout layout, byte[] vertexData, byte[] instanceData, byte[] uniformData, Dictionary<long, BatchSlot> slots) {
        public VertexArrayInstanced VertexArrayInstanced = new VertexArrayInstanced(layout);
        public UniformBuffer UniformBuffer = new UniformBuffer(layout);
        public byte[] VertexData = vertexData;
        public byte[] InstanceData = instanceData;
        public byte[] UniformData = uniformData;
        public Dictionary<long, BatchSlot> Slots = slots;
        public HashSet<long> PreviousSlots = [.. slots.Keys], CurrentSlots = [.. slots.Keys];
    }
    private struct BatchSlot {
        public int Offset;
        public int Size;
    }
}