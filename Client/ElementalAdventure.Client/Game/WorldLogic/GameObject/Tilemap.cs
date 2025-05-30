using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.Resources.HighLevel;
using ElementalAdventure.Client.Game.Components.Data;
using ElementalAdventure.Common.Assets;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.WorldLogic.GameObject;

// TODO: refactor internal format
public class Tilemap {
    private List<Tile> _map;
    private List<Box2> _walls;
    private Vector3i _dimensions;
    private Vector2[] _depth;

    private int _midground;

    private readonly TilemapShaderLayout.GlobalData[] _globalData;
    private TilemapShaderLayout.InstanceData[] _instanceData;

    public int Count => _map.Count;
    public List<Box2> Walls => _walls;
    public int Midground => _midground;

    public Tilemap() {
        _map = [];
        _walls = [];
        _depth = [new(0.0f, 0.0f)];
        _midground = 0;
        _dimensions = new(0, 0, 0);

        _globalData = [new(new(-0.5f, -0.5f, 0.0f)), new(new(0.5f, -0.5f, 0.0f)), new(new(-0.5f, 0.5f, 0.0f)), new(new(0.5f, -0.5f, 0.0f)), new(new(-0.5f, 0.5f, 0.0f)), new(new(0.5f, 0.5f, 0.0f))];
        _instanceData = [];
    }

    public void SetMap(Vector2 depthRange, AssetManager assetManager, AssetID[,,] map, int midground) {
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
                    if (map[z, y, x] != AssetID.None) {
                        int wy = _dimensions.Y - y - 1;
                        TileType type = assetManager.Get<TileType>(map[z, y, x]!);
                        TextureAtlas.Entry value = assetManager.Get<TextureAtlas>(type.TextureAtlas).GetEntry(type.Texture);
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

    public void SetWalls(Box2[] walls) {
        _walls = [];
        for (int i = 0; i < walls.Length; i++) {
            float wy1 = _dimensions.Y - walls[i].Max.Y, wy2 = _dimensions.Y - walls[i].Min.Y;
            _walls.Add(new Box2(new Vector2(walls[i].Min.X - 0.5f, wy1 - 0.5f), new Vector2(walls[i].Max.X - 0.5f, wy2 - 0.5f)));
        }
    }

    public float GetNormalizedDepth(int z, float y, int layerOffset, float heightOffset) {
        return _depth[z + layerOffset].X + (_depth[z + layerOffset].Y - _depth[z + layerOffset].X) * (1.0f - (y + heightOffset + 0.5f) / (_dimensions.Y + 1.0f));
    }

    public void Render(IRenderer renderer) {
        Span<byte> slot = renderer.AllocateInstance(this, 0, new AssetID("shader.tilemap"), new AssetID("textureatlas.dungeon"), MemoryMarshal.Cast<TilemapShaderLayout.GlobalData, byte>(_globalData.AsSpan()), Marshal.SizeOf<TilemapShaderLayout.InstanceData>() * _instanceData.Length);
        MemoryMarshal.Cast<TilemapShaderLayout.InstanceData, byte>(_instanceData.AsSpan()).CopyTo(slot);
    }

    public readonly record struct Tile(Vector3 Position, int Index, Vector2i FrameSize, int FrameCount, int FrameTime);
}