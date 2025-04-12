using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Resource;
using ElementalAdventure.Client.Game.Assets;
using ElementalAdventure.Client.Game.Utils;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Logic;

// TODO: refactor internal format
public class Tilemap {
    private Tile[,,] _map;
    private int _count;
    private bool _dirty;

    public int Count => _count;
    public bool Dirty { get => _dirty; set => _dirty = value; }

    public Tilemap() {
        _map = new Tile[0, 0, 0];
        _count = 0;
        _dirty = true;
    }

    public void SetMap(AssetManager assetManager, string?[,,] map) {
        _map = new Tile[map.GetLength(0), map.GetLength(1), map.GetLength(2)];
        _count = 0;
        _dirty = true;

        float zFactor = MathF.BitDecrement(1.0f / (map.GetLength(0) - 1));
        for (int z = 0; z < map.GetLength(0); z++) {
            for (int y = 0; y < map.GetLength(1); y++) {
                for (int x = 0; x < map.GetLength(2); x++) {
                    if (map[z, y, x] == null) {
                        _map[z, map.GetLength(1) - y - 1, x] = new Tile(false, new Vector3(x, map.GetLength(1) - y - 1, z * zFactor), 0, 0, 0);
                    } else {
                        int wy = map.GetLength(1) - y - 1;
                        TileType type = assetManager.GetTileType(map[z, y, x]!);
                        TextureAtlas<string>.Entry value = assetManager.GetTextureAtlas(type.TextureAtlas).GetEntry(type.Texture);
                        _map[z, wy, x] = new Tile(true, new Vector3(x, wy, z * zFactor), value.Index, value.FrameCount, value.FrameTime);
                        _count++;
                    }
                }
            }
        }
    }

    public GlobalData[] GetGlobalData() {
        return [new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f))];
    }

    public InstanceData[] GetInstanceData() {
        List<InstanceData> data = new(_count);
        for (int z = 0; z < _map.GetLength(0); z++)
            for (int y = 0; y < _map.GetLength(1); y++)
                for (int x = 0; x < _map.GetLength(2); x++)
                    if (_map[z, y, x].Exists)
                        data.Add(new(_map[z, y, x].Position, _map[z, y, x].Index, _map[z, y, x].FrameCount, _map[z, y, x].FrameTime));
        return data.GetBackingArray();
    }

    public readonly record struct Tile(bool Exists, Vector3 Position, int Index, int FrameCount, int FrameTime);

    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct GlobalData(Vector3 position) {
        [FieldOffset(0)] public Vector3 Position = position;
    }

    [StructLayout(LayoutKind.Explicit, Size = 24)]
    public struct InstanceData(Vector3 position, int index, int frameCount, int frameTime) {
        [FieldOffset(0)] public Vector3 Position = position;
        [FieldOffset(12)] public int Index = index;
        [FieldOffset(16)] public int FrameCount = frameCount;
        [FieldOffset(20)] public int FrameTime = frameTime;
    }
}