using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core;
using ElementalAdventure.Client.Core.Assets;
using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.Resources;
using ElementalAdventure.Client.Core.Resources.Composed;
using ElementalAdventure.Client.Game.Data;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Logic.GameObject;

// TODO: refactor internal format
public class Tilemap : IRenderable<string> {
    private List<Tile> _map;
    private Vector3i _dimensions;
    private Vector2[] _depth;

    private int _midground;

    private readonly TilemapShaderLayout.GlobalData[] _globalData;
    private TilemapShaderLayout.InstanceData[] _instanceData;

    public int Count => _map.Count;
    public int Midground => _midground;

    public Tilemap() {
        _map = [];
        _depth = [];
        _dimensions = new(0, 0, 0);

        _globalData = [new(new(-0.5f, -0.5f, 0.0f)), new(new(0.5f, -0.5f, 0.0f)), new(new(-0.5f, 0.5f, 0.0f)), new(new(0.5f, -0.5f, 0.0f)), new(new(-0.5f, 0.5f, 0.0f)), new(new(0.5f, 0.5f, 0.0f))];
        _instanceData = [];
    }

    public void SetMap(Vector2 depthRange, AssetManager<string> assetManager, string?[,,] map, int midground) {
        _map = [];
        _depth = new Vector2[map.GetLength(0)];
        _dimensions = new(map.GetLength(2), map.GetLength(1), map.GetLength(0));

        _midground = midground;

        float factor = 1.0f / _dimensions.Z;
        for (int z = 0; z < _dimensions.Z; z++)
            _depth[z] = new Vector2(depthRange.X + (depthRange.Y - depthRange.X) * z * factor, MathF.BitDecrement(depthRange.X + (depthRange.Y - depthRange.X) * (z + 1) * factor));

        for (int z = 0; z < _dimensions.Z; z++) {
            for (int y = 0; y < _dimensions.Y; y++) {
                for (int x = 0; x < _dimensions.X; x++) {
                    if (map[z, y, x] != null) {
                        int wy = _dimensions.Y - y - 1;
                        TileType type = assetManager.Get<TileType>(map[z, y, x]!);
                        TextureAtlas<string>.Entry value = assetManager.Get<TextureAtlas<string>>(type.TextureAtlas).GetEntry(type.Texture);
                        _map.Add(new Tile(new Vector3(x, wy, GetNormalizedDepth(z, wy, type.DepthLayerOffset, type.DepthHeightOffset)), value.Index, new Vector2i(value.Width, value.Height), value.FrameCount, value.FrameTime));
                    }
                }
            }
        }

        _instanceData = new TilemapShaderLayout.InstanceData[_map.Count];
        for (int i = 0; i < _map.Count; i++) {
            Tile tile = _map[i];
            _instanceData[i] = new(tile.Position, tile.Position, tile.Index, tile.FrameSize, tile.FrameCount, tile.FrameTime);
        }
    }

    public float GetNormalizedDepth(int z, float y, int layerOffset, float heightOffset) {
        return _depth[z + layerOffset].X + (_depth[z + layerOffset].Y - _depth[z + layerOffset].X) * (1.0f - (y + heightOffset + 0.5f) / (_dimensions.Y + 1.0f));
    }

    public RenderCommand<string>[] Render() {
        return [new RenderCommand<string>("shader.tilemap", "textureatlas.dungeon", MemoryMarshal.Cast<TilemapShaderLayout.GlobalData, byte>(_globalData.AsSpan()).ToArray(), MemoryMarshal.Cast<TilemapShaderLayout.InstanceData, byte>(_instanceData.AsSpan()).ToArray())];
    }

    public readonly record struct Tile(Vector3 Position, int Index, Vector2i FrameSize, int FrameCount, int FrameTime);
}