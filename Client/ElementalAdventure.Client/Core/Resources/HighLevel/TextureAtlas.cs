using ElementalAdventure.Client.Core.Assets;
using ElementalAdventure.Client.Core.Resources.OpenGL;

using StbImageSharp;

namespace ElementalAdventure.Client.Core.Resources.Composed;

public class TextureAtlas : IDisposable {
    private readonly Texture2D _atlas;
    private readonly int _cellWidth, _cellHeight, _cellPadding;
    private readonly Dictionary<AssetID, Entry> _entries;

    public int Id => _atlas.Id;
    public int AtlasWidth => _atlas.Width;
    public int AtlasHeight => _atlas.Height;
    public int CellWidth => _cellWidth;
    public int CellHeight => _cellHeight;
    public int CellPadding => _cellPadding;
    public Dictionary<AssetID, Entry> Entries => _entries;

    public TextureAtlas(Dictionary<AssetID, EntryDef> entries, int padding) {
        if (entries.Count == 0)
            throw new ArgumentException("TextureAtlas must contain at least one tile.");
        foreach (KeyValuePair<AssetID, EntryDef> entry in entries)
            if (entry.Value.Frames.Length == 0)
                throw new ArgumentException("TextureAtlas entry must contain at least one frame.");

        _cellPadding = padding;
        _cellWidth = 0;
        _cellHeight = 0;

        int count = 0;
        foreach (KeyValuePair<AssetID, EntryDef> entry in entries) {
            int frameWidth = -1, frameHeight = -1;
            for (int i = 0; i < entry.Value.Frames.Length; i++) {
                ImageResult frame = ImageResult.FromMemory(entry.Value.Frames[i], ColorComponents.RedGreenBlueAlpha);
                if (frameWidth == -1 && frameHeight == -1)
                    (frameWidth, frameHeight) = (frame.Width, frame.Height);
                else if (frameWidth != frame.Width || frameHeight != frame.Height)
                    throw new ArgumentException("All frames of a TextureAtlas entry must have the same dimensions.");
            }
            count += entry.Value.Frames.Length;
            _cellWidth = Math.Max(_cellWidth, frameWidth); _cellHeight = Math.Max(_cellHeight, frameHeight);
        }
        int paddedCellWidth = _cellWidth + 2 * _cellPadding, paddedCellHeight = _cellHeight + 2 * _cellPadding;
        int atlasCols = (int)Math.Ceiling(Math.Sqrt(count)), atlasRows = (int)Math.Ceiling(count / Math.Ceiling(Math.Sqrt(count)));
        int atlasWidth = atlasCols * paddedCellWidth, atlasHeight = atlasRows * paddedCellHeight;

        _entries = new(entries.Count);

        byte[] data = new byte[atlasWidth * atlasHeight * 4];
        int index = 0;
        foreach (KeyValuePair<AssetID, EntryDef> entry in entries) {
            int entryIndex = index, entryWidth = -1, entryHeight = -1;
            for (int i = 0; i < entry.Value.Frames.Length; i++) {
                ImageResult frame = ImageResult.FromMemory(entry.Value.Frames[i], ColorComponents.RedGreenBlueAlpha);

                if (entryWidth == -1 && entryHeight == -1)
                    (entryWidth, entryHeight) = (frame.Width, frame.Height);
                int col = index % atlasCols, row = index / atlasCols;
                int offsetPixelsX = col * paddedCellWidth + _cellPadding, offsetPixelsY = row * paddedCellHeight + _cellPadding;

                for (int y = 0; y < frame.Height; y++) {
                    int src = y * frame.Width * 4;
                    int dst = ((offsetPixelsY + y) * atlasWidth + offsetPixelsX) * 4;
                    int cnt = frame.Width * 4;
                    Buffer.BlockCopy(frame.Data, src, data, dst, cnt);

                    for (int x = 0; x < _cellPadding; x++) {
                        int srcLeft = y * frame.Width * 4;
                        int dstLeft = ((offsetPixelsY + y) * atlasWidth + offsetPixelsX - 1 - x) * 4;
                        int cntLeft = 4;
                        Buffer.BlockCopy(frame.Data, srcLeft, data, dstLeft, cntLeft);

                        int srcRight = (y * frame.Width + frame.Width - 1) * 4;
                        int dstRight = ((offsetPixelsY + y) * atlasWidth + offsetPixelsX + frame.Width + x) * 4;
                        int cntRight = 4;
                        Buffer.BlockCopy(frame.Data, srcRight, data, dstRight, cntRight);
                    }
                }
                for (int y = 0; y < _cellPadding; y++) {
                    int srcTop = (offsetPixelsY * atlasWidth + offsetPixelsX - _cellPadding) * 4;
                    int dstTop = ((offsetPixelsY - 1 - y) * atlasWidth + offsetPixelsX - _cellPadding) * 4;
                    int cntTop = (frame.Width + _cellPadding * 2) * 4;
                    Buffer.BlockCopy(data, srcTop, data, dstTop, cntTop);

                    int srcBottom = ((offsetPixelsY + frame.Height - 1) * atlasWidth + offsetPixelsX - _cellPadding) * 4;
                    int dstBottom = ((offsetPixelsY + frame.Height + y) * atlasWidth + offsetPixelsX - _cellPadding) * 4;
                    int cntBottom = (frame.Width + _cellPadding * 2) * 4;
                    Buffer.BlockCopy(data, srcBottom, data, dstBottom, cntBottom);
                }
                index++;
            }
            _entries[entry.Key] = new Entry(entryIndex, entryWidth, entryHeight, entry.Value.Frames.Length, entry.Value.FrameTime);
        }

        _atlas = new(data, atlasWidth, atlasHeight);
    }

    public Entry GetEntry(AssetID key) {
        return _entries[key];
    }

    public void Dispose() {
        _atlas.Dispose();
        GC.SuppressFinalize(this);
    }

    public record struct EntryDef(byte[][] Frames, int FrameTime);
    public record struct Entry(int Index, int Width, int Height, int FrameCount, int FrameTime);
}