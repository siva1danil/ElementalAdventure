using ElementalAdventure.Client.Core.Assets;
using ElementalAdventure.Client.Core.Resource;
using ElementalAdventure.Client.Game.Data;
using ElementalAdventure.Client.Game.Utils;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Logic.GameObject;

// TODO: refactor internal format
public class Tilemap {
    private Tile[,,] _map;
    private Vector2[] _depth;

    private int _count;
    private int _midground;
    private bool _dirty;

    public int Count => _count;
    public int Midground => _midground;
    public bool Dirty { get => _dirty; set => _dirty = value; }

    public Tilemap() {
        _map = new Tile[0, 0, 0];
        _depth = [];

        _count = 0;
        _dirty = true;
    }

    public void SetMap(Vector2 depthRange, AssetManager<string> assetManager, string?[,,] map, int midground) {
        _map = new Tile[map.GetLength(0), map.GetLength(1), map.GetLength(2)];
        _depth = new Vector2[map.GetLength(0)];

        _count = 0;
        _midground = midground;
        _dirty = true;

        float factor = 1.0f / map.GetLength(0);
        for (int z = 0; z < map.GetLength(0); z++)
            _depth[z] = new Vector2(depthRange.X + (depthRange.Y - depthRange.X) * z * factor, MathF.BitDecrement(depthRange.X + (depthRange.Y - depthRange.X) * (z + 1) * factor));

        for (int z = 0; z < map.GetLength(0); z++) {
            for (int y = 0; y < map.GetLength(1); y++) {
                for (int x = 0; x < map.GetLength(2); x++) {
                    if (map[z, y, x] == null) {
                        _map[z, map.GetLength(1) - y - 1, x] = new Tile(false, new Vector3(x, map.GetLength(1) - y - 1, GetNormalizedDepth(z, map.GetLength(1) - y - 1, 0, 0.0f)), 0, 0, 0);
                    } else {
                        int wy = map.GetLength(1) - y - 1;
                        TileType type = assetManager.Get<TileType>(map[z, y, x]!);
                        TextureAtlas<string>.Entry value = assetManager.Get<TextureAtlas<string>>(type.TextureAtlas).GetEntry(type.Texture);
                        _map[z, wy, x] = new Tile(true, new Vector3(x, wy, GetNormalizedDepth(z, wy, type.DepthLayerOffset, type.DepthHeightOffset)), value.Index, value.FrameCount, value.FrameTime);
                        _count++;
                    }
                }
            }
        }
    }

    public float GetNormalizedDepth(int z, float y, int layerOffset, float heightOffset) {
        return _depth[z + layerOffset].X + (_depth[z + layerOffset].Y - _depth[z + layerOffset].X) * (1.0f - (y + heightOffset) / _map.GetLength(1));
    }

    public TilemapShaderLayout.GlobalData[] GetGlobalData() {
        return [new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f))];
    }

    public TilemapShaderLayout.InstanceData[] GetInstanceData() {
        List<TilemapShaderLayout.InstanceData> data = new(_count);
        for (int z = 0; z < _map.GetLength(0); z++)
            for (int y = 0; y < _map.GetLength(1); y++)
                for (int x = 0; x < _map.GetLength(2); x++)
                    if (_map[z, y, x].Exists)
                        data.Add(new(_map[z, y, x].Position, _map[z, y, x].Position, _map[z, y, x].Index, _map[z, y, x].FrameCount, _map[z, y, x].FrameTime));
        return data.GetBackingArray();
    }

    public readonly record struct Tile(bool Exists, Vector3 Position, int Index, int FrameCount, int FrameTime);
}