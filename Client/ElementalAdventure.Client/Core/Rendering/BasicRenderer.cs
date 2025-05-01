using ElementalAdventure.Client.Core.Assets;
using ElementalAdventure.Client.Core.Resources.Composed;
using ElementalAdventure.Client.Core.Resources.OpenGL;

using OpenTK.Graphics.OpenGL4;

namespace ElementalAdventure.Client.Core.Rendering;

public class BasicRenderer<K> : IDisposable where K : notnull {
    private readonly AssetManager<K> _assetManager;
    private readonly IUniformProvider<K> _uniformProvider;

    private readonly Dictionary<UniformKey, UniformBuffer> _uniformBuffers;
    private readonly Dictionary<Key, List<byte>> _vertexData;
    private readonly Dictionary<Key, List<byte>> _instanceData;
    private readonly Dictionary<Key, VertexArrayInstanced> _vertexArrays;
    private readonly HashSet<Key> _queue;

    public BasicRenderer(AssetManager<K> assetManager, IUniformProvider<K> uniformProvider) {
        _assetManager = assetManager;
        _uniformProvider = uniformProvider;

        _uniformBuffers = [];
        _vertexData = [];
        _instanceData = [];
        _vertexArrays = [];
        _queue = [];
    }

    public void Enqueue(IRenderable<K> renderable) {
        foreach (RenderCommand<K> command in renderable.Render()) {
            UniformKey ukey = new(command.ShaderProgram, command.TextureAtlas);
            Key key = new(command.ShaderProgram, command.TextureAtlas, MurmurHash64A(command.VertexData));
            if (!_uniformBuffers.ContainsKey(ukey)) {
                _uniformBuffers[ukey] = new UniformBuffer(_assetManager.Get<ShaderProgram>(command.ShaderProgram).Layout);
            }
            if (!_vertexData.ContainsKey(key)) {
                _vertexData[key] = [.. command.VertexData];
                _instanceData[key] = [];
                _vertexArrays[key] = new VertexArrayInstanced(_assetManager.Get<ShaderProgram>(command.ShaderProgram).Layout);
            }
            _instanceData[key].AddRange(command.InstanceData);
            _queue.Add(key);
        }
    }

    public void Flush() {
        int lastShader = -1, lastTexture = -1;
        foreach (Key key in _queue) {
            UniformKey ukey = new(key.ShaderProgram, key.TextureAtlas);
            int shader = _assetManager.Get<ShaderProgram>(key.ShaderProgram).Id, texture = _assetManager.Get<TextureAtlas<K>>(key.TextureAtlas).Id;

            if (shader != lastShader) {
                GL.UseProgram(shader);
            }
            if (texture != lastTexture) {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, _assetManager.Get<TextureAtlas<K>>(key.TextureAtlas).Id);
            }
            if (shader != lastShader || texture != lastTexture) {
                _uniformBuffers[ukey].SetData(_uniformProvider.GetUniformData(key.ShaderProgram, key.TextureAtlas));
                GL.BindBuffer(BufferTarget.UniformBuffer, _uniformBuffers[ukey].Id);
                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, _uniformBuffers[ukey].Id);
            }

            _vertexArrays[key].SetGlobalData([.. _vertexData[key]], BufferUsageHint.DynamicDraw);
            _vertexArrays[key].SetInstanceData([.. _instanceData[key]], BufferUsageHint.DynamicDraw);

            GL.BindVertexArray(_vertexArrays[key].Id);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, _vertexData[key].Count / _vertexArrays[key].VertexDataSize, _instanceData[key].Count / _vertexArrays[key].InstanceDataSize);

            _instanceData[key].Clear();
        }
        _queue.Clear();
    }

    public void Dispose() {
        foreach (var buffer in _uniformBuffers.Values)
            buffer.Dispose();
        foreach (var vao in _vertexArrays.Values)
            vao.Dispose();
        _uniformBuffers.Clear();
        _vertexData.Clear();
        _instanceData.Clear();
        _vertexArrays.Clear();
        GC.SuppressFinalize(this);
    }

    private static ulong MurmurHash64A(byte[] data, ulong seed = 0UL) {
        ArgumentNullException.ThrowIfNull(data);

        const ulong m = 0xc6a4a7935bd1e995UL;
        const int r = 47;

        int length = data.Length;
        ulong h = seed ^ ((ulong)length * m);

        int position = 0;
        int limit = length - 8;

        while (position <= limit) {
            ulong k = BitConverter.ToUInt64(data, position);
            k *= m;
            k ^= k >> r;
            k *= m;

            h ^= k;
            h *= m;

            position += 8;
        }

        int tailBytes = length - position;
        if (tailBytes > 0) {
            ulong tail = 0UL;
            for (int i = 0; i < tailBytes; i++) {
                tail |= (ulong)data[position + i] << (8 * i);
            }
            h ^= tail;
            h *= m;
        }

        h ^= h >> r;
        h *= m;
        h ^= h >> r;

        return h;
    }

    private readonly record struct Key(K ShaderProgram, K TextureAtlas, ulong VertexHash);
    private readonly record struct UniformKey(K ShaderProgram, K TextureAtlas);
}