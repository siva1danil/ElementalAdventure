using ElementalAdventure.Client.Core.OpenGL;

using StbImageSharp;

namespace ElementalAdventure.Client.Core.Resource;

public class TextureAtlas<K> : IDisposable where K : notnull {
    private readonly Texture2D _atlas;
    private readonly int _entryWidth, _entryHeight, _entryPadding;
    private readonly Dictionary<K, Entry> _entries;

    public int Id => _atlas.Id;
    public int AtlasWidth => _atlas.Width;
    public int AtlasHeight => _atlas.Height;
    public int EntryWidth => _entryWidth;
    public int EntryHeight => _entryHeight;
    public int EntryPadding => _entryPadding;
    public Dictionary<K, Entry> Entries => _entries;

    public TextureAtlas(Dictionary<K, EntryDef> entries, int padding) {
        if (entries.Count == 0)
            throw new ArgumentException("TextureAtlas must contain at least one tile.");
        foreach (KeyValuePair<K, EntryDef> entry in entries)
            if (entry.Value.Frames.Length == 0)
                throw new ArgumentException("TextureAtlas entry must contain at least one frame.");

        _entryPadding = padding;

        int count = 0;
        _entryWidth = -1; _entryHeight = -1;
        foreach (KeyValuePair<K, EntryDef> entry in entries) {
            for (int i = 0; i < entry.Value.Frames.Length; i++) {
                ImageResult frame = ImageResult.FromMemory(entry.Value.Frames[i], ColorComponents.RedGreenBlueAlpha);
                if (_entryWidth == -1 && _entryHeight == -1)
                    (_entryWidth, _entryHeight) = (frame.Width, frame.Height);
                else if (_entryWidth != frame.Width || _entryHeight != frame.Height)
                    throw new ArgumentException("All entries and frames in a TextureAtlas must have the same dimensions.");
            }
            count += entry.Value.Frames.Length;
        }
        int paddedWidth = _entryWidth + 2 * _entryPadding, paddedHeight = _entryHeight + 2 * _entryPadding;
        int atlasCols = (int)Math.Ceiling(Math.Sqrt(count)), atlasRows = (int)Math.Ceiling(count / Math.Ceiling(Math.Sqrt(count)));
        int atlasWidth = atlasCols * paddedWidth, atlasHeight = atlasRows * paddedHeight;

        _entries = new(entries.Count);

        byte[] data = new byte[atlasWidth * atlasHeight * 4];
        int index = 0;
        foreach (KeyValuePair<K, EntryDef> entry in entries) {
            _entries[entry.Key] = new Entry(index, entry.Value.Frames.Length, entry.Value.FrameTime);
            for (int i = 0; i < entry.Value.Frames.Length; i++) {
                ImageResult frame = ImageResult.FromMemory(entry.Value.Frames[i], ColorComponents.RedGreenBlueAlpha);
                int col = index % atlasCols, row = index / atlasCols;
                int offsetX = col * paddedWidth + _entryPadding, offsetY = row * paddedHeight + _entryPadding;
                for (int y = -_entryPadding; y < _entryHeight + _entryPadding; y++) {
                    int clampedY = Math.Clamp(y, 0, _entryHeight - 1);
                    for (int x = -_entryPadding; x < _entryWidth + _entryPadding; x++) {
                        int clampedX = Math.Clamp(x, 0, _entryWidth - 1);
                        int srcIndex = (clampedY * _entryWidth + clampedX) * 4;
                        int dstX = offsetX + x;
                        int dstY = offsetY + y;
                        int dstIndex = (dstY * atlasWidth + dstX) * 4;
                        Buffer.BlockCopy(frame.Data, srcIndex, data, dstIndex, 4);
                    }
                }
                index++;
            }
        }

        _atlas = new(data, atlasWidth, atlasHeight);
    }

    public Entry GetEntry(K key) {
        return _entries[key];
    }

    public void Dispose() {
        _atlas.Dispose();
        GC.SuppressFinalize(this);
    }

    public record struct EntryDef(byte[][] Frames, int FrameTime);
    public record struct Entry(int Index, int FrameCount, int FrameTime);
}