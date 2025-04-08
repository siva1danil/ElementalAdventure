using ElementalAdventure.Client.Core.OpenGL;

using OpenTK.Mathematics;

using StbImageSharp;

namespace ElementalAdventure.Client.Core.Resource;

public class Tileset : IDisposable {
    private readonly Texture2D _atlas;
    private readonly TileType[] _tiles;

    public Texture2D Atlas => _atlas;
    public TileType[] Tiles => _tiles;

    public Tileset(TileDef[] tiles) {
        if (tiles.Length == 0)
            throw new ArgumentException("Tileset must contain at least one tile.");
        for (int i = 0; i < tiles.Length; i++)
            if (tiles[i].Frames.Length == 0)
                throw new ArgumentException("Tileset tile must contain at least one frame.");

        ImageResult first = ImageResult.FromMemory(tiles[0].Frames[0], ColorComponents.RedGreenBlueAlpha);
        int tileCount = 0;
        for (int i = 0; i < tiles.Length; i++)
            tileCount += tiles[i].Frames.Length;
        Vector2i tileSize = new(first.Width, first.Height);
        Vector2i atlasTiles = new((int)Math.Ceiling(Math.Sqrt(tileCount)), (int)Math.Ceiling(tileCount / Math.Ceiling(Math.Sqrt(tileCount))));
        Vector2i atlasSize = new(atlasTiles.X * tileSize.X, atlasTiles.Y * tileSize.Y);

        _tiles = new TileType[tiles.Length];
        byte[] data = new byte[atlasSize.X * atlasSize.Y * 4];

        int index = 0;
        for (int i = 0; i < tiles.Length; i++) {
            _tiles[i] = new TileType { Index = i, FrameCount = tiles[i].Frames.Length, FrameTime = tiles[i].FrameTime };
            for (int j = 0; j < tiles[i].Frames.Length; j++) {
                ImageResult frame = ImageResult.FromMemory(tiles[i].Frames[j], ColorComponents.RedGreenBlueAlpha);
                Vector2i rowcol = new(index % atlasTiles.X, index / atlasTiles.X);
                int offsetX = rowcol.X * tileSize.X;
                int offsetY = rowcol.Y * tileSize.Y;
                for (int y = 0; y < tileSize.Y; y++) {
                    int srcStart = y * tileSize.X * 4;
                    int dstStart = ((offsetY + y) * atlasSize.X + offsetX) * 4;
                    Buffer.BlockCopy(frame.Data, srcStart, data, dstStart, tileSize.X * 4);
                }
                index++;
            }
        }

        _atlas = new(data, atlasSize.X, atlasSize.Y);
    }

    public void Dispose() {
        _atlas.Dispose();
        GC.SuppressFinalize(this);
    }

    public struct TileDef {
        public byte[][] Frames;
        public int FrameTime;
    }

    public struct TileType {
        public int Index;
        public int FrameCount;
        public int FrameTime;
    }
}