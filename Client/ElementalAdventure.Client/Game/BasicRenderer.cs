using ElementalAdventure.Client.Core.Assets;
using ElementalAdventure.Client.Core.OpenGL;
using ElementalAdventure.Client.Core.Resource;
using ElementalAdventure.Client.Game.Data;
using ElementalAdventure.Client.Game.Utils;

using OpenTK.Graphics.OpenGL4;

namespace ElementalAdventure.Client.Game;

public class BasicRenderer : IDisposable {
    private readonly AssetManager<string> _assetManager;
    private readonly Dictionary<BatchKey, BatchValue> _batches;

    public BasicRenderer(AssetManager<string> assetManager) {
        _assetManager = assetManager;
        _batches = [];
    }

    public void SetUniform(string shaderProgram, string textureAtlas, Span<byte> data) {
        BatchKey key = new(shaderProgram, textureAtlas);
        if (!_batches.ContainsKey(key))
            _batches[key] = new BatchValue(_assetManager.Get<ShaderLayout>(shaderProgram));
        _batches[key].UniformBuffer.SetData(data.ToArray());
    }

    public void Enqueue(string shaderProgram, string textureAtlas, Span<byte> globalData, Span<byte> instanceData) {
        BatchKey key = new(shaderProgram, textureAtlas);
        int hash = Hash(globalData);
        if (!_batches.ContainsKey(key)) {
            _batches[key] = new BatchValue(_assetManager.Get<ShaderLayout>(shaderProgram));
        }
        if (!_batches[key].VertexArrays.ContainsKey(hash)) {
            _batches[key].VertexArrays[hash] = new BatchData(new VertexArrayInstanced(_batches[key].ShaderLayout.GlobalType, _batches[key].ShaderLayout.InstanceType));
            _batches[key].VertexArrays[hash].GlobalData.AddRange(globalData);
            _batches[key].VertexArrays[hash].GlobalCount = globalData.Length;
        }
        _batches[key].VertexArrays[hash].InstanceData.AddRange(instanceData);
        _batches[key].VertexArrays[hash].InstanceCount += instanceData.Length;
    }

    public void Render() {
        int shader = -1, texture = -1;
        foreach (KeyValuePair<BatchKey, BatchValue> batch in _batches) {
            int batchShader = _assetManager.Get<ShaderProgram>(batch.Key.ShaderProgram).Id, batchTexture = _assetManager.Get<TextureAtlas<string>>(batch.Key.TextureAtlas).Id;

            if (shader != batchShader) {
                GL.UseProgram(_assetManager.Get<ShaderProgram>(batch.Key.ShaderProgram).Id);
            }
            if (texture != batchTexture) {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, _assetManager.Get<TextureAtlas<string>>(batch.Key.TextureAtlas).Id);
            }
            if (shader != batchShader || texture != batchTexture) {
                GL.BindBuffer(BufferTarget.UniformBuffer, batch.Value.UniformBuffer.Id);
                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, batch.Value.UniformBuffer.Id);
            }

            foreach (KeyValuePair<int, BatchData> vertexArray in batch.Value.VertexArrays) {
                if (vertexArray.Value.InstanceCount == 0)
                    continue;
                vertexArray.Value.VertexArray.SetGlobalData(vertexArray.Value.GlobalData.GetBackingArray(), BufferUsageHint.DynamicDraw);
                vertexArray.Value.VertexArray.SetInstanceData(vertexArray.Value.InstanceData.GetBackingArray(), BufferUsageHint.DynamicDraw);
                GL.BindVertexArray(vertexArray.Value.VertexArray.Id);
                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, vertexArray.Value.GlobalData.Count / vertexArray.Value.VertexArray.GlobalStride, vertexArray.Value.InstanceData.Count / vertexArray.Value.VertexArray.InstanceStride);
                vertexArray.Value.InstanceData.Clear();
                vertexArray.Value.InstanceCount = 0;
            }

            shader = batchShader;
            texture = batchTexture;
        }
    }

    private static int Hash(Span<byte> data) {
        int hash = 17;
        for (int i = 0; i < data.Length; i++)
            hash = hash * 31 + data[i];
        return hash;
    }

    public void Dispose() {
        foreach (KeyValuePair<BatchKey, BatchValue> batch in _batches) {
            foreach (KeyValuePair<int, BatchData> vertexArray in batch.Value.VertexArrays) {
                vertexArray.Value.VertexArray.Dispose();
            }
            batch.Value.UniformBuffer.Dispose();
        }
        _batches.Clear();
        GC.SuppressFinalize(this);
    }

    private readonly record struct BatchKey(string ShaderProgram, string TextureAtlas);
    private class BatchValue(ShaderLayout layout) {
        public readonly ShaderLayout ShaderLayout = layout;
        public readonly Dictionary<int, BatchData> VertexArrays = [];
        public readonly UniformBuffer UniformBuffer = new(layout.UniformType);
    }
    private class BatchData(VertexArrayInstanced vertexArray) {
        public readonly List<byte> GlobalData = [];
        public readonly List<byte> InstanceData = [];
        public readonly VertexArrayInstanced VertexArray = vertexArray;
        public int GlobalCount = 0;
        public int InstanceCount = 0;
    }
}