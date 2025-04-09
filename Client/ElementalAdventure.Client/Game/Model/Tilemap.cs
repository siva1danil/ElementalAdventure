using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Resource;
using ElementalAdventure.Client.Game.Utils;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Model;

public class Tilemap {
    private int[,] _map;
    private int _tileCount;
    private bool _dirty;

    public int TileCount => _tileCount;
    public bool Dirty => _dirty;

    public Tilemap() {
        _map = new int[0, 0];
    }

    public void SetMap<T>(TextureAtlas<T> atlas, T?[,] map) where T : notnull {
        _map = new int[map.GetLength(0), map.GetLength(1)];
        _tileCount = 0;
        _dirty = true;
        for (int y = 0; y < map.GetLength(0); y++) {
            for (int x = 0; x < map.GetLength(1); x++) {
                _map[y, x] = map[map.GetLength(0) - y - 1, x] == null ? 0 : atlas.GetEntry(map[map.GetLength(0) - y - 1, x]!).Index + 1;
                if (_map[y, x] != 0) _tileCount++;
            }
        }
    }

    public void SetMap(int[,] map) {
        _map = new int[map.GetLength(0), map.GetLength(1)];
        _tileCount = 0;
        _dirty = true;
        for (int y = 0; y < map.GetLength(0); y++) {
            for (int x = 0; x < map.GetLength(1); x++) {
                _map[y, x] = map[map.GetLength(0) - y - 1, x];
                if (_map[y, x] != 0) _tileCount++;
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
                if (_map[y, x] != 0) data.Add(new InstanceData(new(x, y, 0), _map[y, x] - 1));
        return data.GetBackingArray();
    }

    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct VertexData(Vector3 position) {
        [FieldOffset(0)] public Vector3 Position = position;
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct InstanceData(Vector3 position, int index) {
        [FieldOffset(0)] public Vector3 Position = position;
        [FieldOffset(12)] public int Index = index;
    }
}