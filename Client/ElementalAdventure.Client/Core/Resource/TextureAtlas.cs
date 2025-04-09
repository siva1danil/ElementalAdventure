using ElementalAdventure.Client.Core.OpenGL;

using OpenTK.Mathematics;

using StbImageSharp;

namespace ElementalAdventure.Client.Core.Resource;

public class TextureAtlas<K> : IDisposable where K : notnull {
    private readonly Texture2D _atlas;
    private readonly int _entryWidth, _entryHeight;
    private readonly Dictionary<K, Entry> _entries;

    public int Id => _atlas.Id;
    public int AtlasWidth => _atlas.Width;
    public int AtlasHeight => _atlas.Height;
    public int EntryWidth => _entryWidth;
    public int EntryHeight => _entryHeight;
    public Dictionary<K, Entry> Entries => _entries;

    public TextureAtlas(Dictionary<K, EntryDef> entries) {
        if (entries.Count == 0)
            throw new ArgumentException("Tileset must contain at least one tile.");
        foreach (KeyValuePair<K, EntryDef> entry in entries)
            if (entry.Value.Frames.Length == 0)
                throw new ArgumentException("Tileset tile must contain at least one frame.");

        int entryCount = 0;
        foreach (KeyValuePair<K, EntryDef> entry in entries)
            entryCount += entry.Value.Frames.Length;

        Dictionary<K, EntryDef>.Enumerator enumerator = entries.GetEnumerator();
        enumerator.MoveNext();

        ImageResult first = ImageResult.FromMemory(enumerator.Current.Value.Frames[0], ColorComponents.RedGreenBlueAlpha);
        (_entryWidth, _entryHeight) = (first.Width, first.Height);
        Vector2i atlasTiles = new((int)Math.Ceiling(Math.Sqrt(entryCount)), (int)Math.Ceiling(entryCount / Math.Ceiling(Math.Sqrt(entryCount))));
        Vector2i atlasSize = new(atlasTiles.X * _entryWidth, atlasTiles.Y * _entryHeight);

        _entries = new(entries.Count);
        byte[] data = new byte[atlasSize.X * atlasSize.Y * 4];
        int index = 0;
        foreach (KeyValuePair<K, EntryDef> entry in entries) {
            _entries[entry.Key] = new Entry(index, entry.Value.Frames.Length, entry.Value.FrameTime);
            for (int i = 0; i < entry.Value.Frames.Length; i++) {
                ImageResult frame = ImageResult.FromMemory(entry.Value.Frames[i], ColorComponents.RedGreenBlueAlpha);
                Vector2i rowcol = new(index % atlasTiles.X, index / atlasTiles.X);
                Vector2i offset = new(rowcol.X * _entryWidth, rowcol.Y * _entryHeight);
                for (int y = 0; y < _entryHeight; y++) {
                    int srcStart = y * _entryWidth * 4;
                    int dstStart = ((offset.Y + y) * atlasSize.X + offset.X) * 4;
                    Buffer.BlockCopy(frame.Data, srcStart, data, dstStart, _entryWidth * 4);
                }
                index++;
            }
        }
        _atlas = new(data, atlasSize.X, atlasSize.Y);
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