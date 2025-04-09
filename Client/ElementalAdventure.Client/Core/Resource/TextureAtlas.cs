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
            throw new ArgumentException("Tileset must contain at least one tile.");
        foreach (KeyValuePair<K, EntryDef> entry in entries)
            if (entry.Value.Frames.Length == 0)
                throw new ArgumentException("Tileset tile must contain at least one frame.");

        _entryPadding = padding;

        int entryCount = 0;
        foreach (KeyValuePair<K, EntryDef> entry in entries)
            entryCount += entry.Value.Frames.Length;

        Dictionary<K, EntryDef>.Enumerator enumerator = entries.GetEnumerator();
        enumerator.MoveNext();

        ImageResult first = ImageResult.FromMemory(enumerator.Current.Value.Frames[0], ColorComponents.RedGreenBlueAlpha);
        (_entryWidth, _entryHeight) = (first.Width, first.Height);

        (int paddedWidth, int paddedHeight) = (_entryWidth + 2 * _entryPadding, _entryHeight + 2 * _entryPadding);
        (int atlasCols, int atlasRows) = ((int)Math.Ceiling(Math.Sqrt(entryCount)), (int)Math.Ceiling(entryCount / Math.Ceiling(Math.Sqrt(entryCount))));
        (int atlasWidth, int atlasHeight) = (atlasCols * paddedWidth, atlasRows * paddedHeight);

        _entries = new(entries.Count);
        byte[] data = new byte[atlasWidth * atlasHeight * 4];
        int index = 0;
        foreach (KeyValuePair<K, EntryDef> entry in entries) {
            _entries[entry.Key] = new Entry(index, entry.Value.Frames.Length, entry.Value.FrameTime);
            for (int i = 0; i < entry.Value.Frames.Length; i++) {
                ImageResult frame = ImageResult.FromMemory(entry.Value.Frames[i], ColorComponents.RedGreenBlueAlpha);
                (int col, int row) = (index % atlasCols, index / atlasCols);
                (int offsetX, int offsetY) = (col * paddedWidth + _entryPadding, row * paddedHeight + _entryPadding);
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

    public struct EntryDef(byte[][] frames, int frameTime) {
        public byte[][] Frames = frames;
        public int FrameTime = frameTime;
    }

    public struct Entry(int index, int frameCount, int frameTime) {
        public int Index = index;
        public int FrameCount = frameCount;
        public int FrameTime = frameTime;
    }
}