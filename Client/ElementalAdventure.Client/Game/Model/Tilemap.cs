using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Resource;
using ElementalAdventure.Client.Game.Utils;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Model;

public class Tilemap {
    private Tile?[,] _map;
    private int _tileCount;

    public int TileCount => _tileCount;

    public Tilemap() {
        _map = new Tile?[0, 0];
    }

    public void SetMap<T>(TextureAtlas<T> atlas, T?[,] map) where T : notnull {
        _map = new Tile?[map.GetLength(0), map.GetLength(1)];
        _tileCount = 0;
        for (int y = 0; y < map.GetLength(0); y++) {
            for (int x = 0; x < map.GetLength(1); x++) {
                TextureAtlas<T>.Entry? tile = map[map.GetLength(0) - y - 1, x] == null ? null : atlas.GetEntry(map[map.GetLength(0) - y - 1, x]!);
                if (tile != null) {
                    _map[y, x] = new Tile(tile.Value.Index, tile.Value.FrameCount, tile.Value.FrameTime);
                    _tileCount++;
                }
            }
        }
    }

    public VertexData[] GetVertexData() {
        return [new VertexData(new Vector3(0, 0, 0)), new VertexData(new Vector3(1, 0, 0)), new VertexData(new Vector3(0, 1, 0)), new VertexData(new Vector3(1, 0, 0)), new VertexData(new Vector3(0, 1, 0)), new VertexData(new Vector3(1, 1, 0))];
    }

    public InstanceData[] GetInstanceData() {
        List<InstanceData> data = new List<InstanceData>();
        for (int y = 0; y < _map.GetLength(0); y++)
            for (int x = 0; x < _map.GetLength(1); x++)
                if (_map[y, x] != null) data.Add(new InstanceData(new(x, y, 0), _map[y, x]!.Value.Index, _map[y, x]!.Value.FrameCount, _map[y, x]!.Value.FrameTime));
        return data.GetBackingArray();
    }

    private struct Tile(int index, int frameCount, int frameTime) {
        public int Index = index;
        public int FrameCount = frameCount;
        public int FrameTime = frameTime;
    }

    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct VertexData(Vector3 position) {
        [FieldOffset(0)] public Vector3 Position = position;
    }

    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct InstanceData(Vector3 position, int index, int frameCount, int frameTime) {
        [FieldOffset(0)] public Vector3 Position = position;
        [FieldOffset(12)] public int Index = index;
        [FieldOffset(16)] public int FrameCount = frameCount;
        [FieldOffset(20)] public int FrameTime = frameTime;
    }
}