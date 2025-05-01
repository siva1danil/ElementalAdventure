using ElementalAdventure.Client.Core.Resources.OpenGL;

using TrueTypeSharp;

namespace ElementalAdventure.Client.Core.Resources;

public class Font : IDisposable {
    private readonly Texture2D _atlas;
    private readonly Dictionary<char, Glyph> _glyphs;

    public int Id => _atlas.Id;
    public Dictionary<char, Glyph> Glyphs => _glyphs;

    public Font(byte[] data, Range[] ranges, int height, int padding) {
        if (ranges.Length == 0)
            throw new ArgumentException("TrueTypeFont must contain at least one range.");

        _glyphs = [];

        TrueTypeFont font = new(data, 0);
        float scale = font.GetScaleForPixelHeight(height);
        int count = 0, maxWidth = 0, maxHeight = 0;
        for (int i = 0; i < ranges.Length; i++) {
            for (char c = ranges[i].A; c <= ranges[i].B; c++) {
                font.GetCodepointBitmapBox(c, scale, scale, out int x0, out int y0, out int x1, out int y1);
                maxWidth = Math.Max(maxWidth, x1 - x0);
                maxHeight = Math.Max(maxHeight, y1 - y0);
            }
            count += ranges[i].B - ranges[i].A + 1;
        }
        int paddedWidth = maxWidth + padding * 2, paddedHeight = maxHeight + padding * 2;
        int atlasCols = (int)Math.Ceiling(Math.Sqrt(count)), atlasRows = (int)Math.Ceiling(count / Math.Ceiling(Math.Sqrt(count)));
        int atlasWidth = atlasCols * paddedWidth, atlasHeight = atlasRows * paddedHeight;

        FontBitmap bitmap = new(atlasWidth, atlasHeight);
        for (int i = 0; i < ranges.Length; i++) {
            BakedChar[] baked = new BakedChar[ranges[i].B - ranges[i].A + 1];
            int result = font.BakeFontBitmap(height, ranges[i].A, baked, bitmap);
            if (result <= 0)
                throw new Exception($"Failed to bake font bitmap: {result}");
            for (int j = 0; j < baked.Length; j++) {
                char c = (char)(ranges[i].A + j);
                BakedChar b = baked[j];
                _glyphs[c] = new Glyph(b.X0, b.Y0, b.X1, b.Y1, b.XOffset, b.YOffset, b.XAdvance);
            }
        }

        byte[] rgba = new byte[atlasWidth * atlasHeight * 4];
        for (int i = 0; i < bitmap.Buffer.Length; i++) {
            byte a = bitmap.Buffer[i];
            rgba[i * 4] = 255;
            rgba[i * 4 + 1] = 255;
            rgba[i * 4 + 2] = 255;
            rgba[i * 4 + 3] = a;
        }

        _atlas = new Texture2D(rgba, atlasWidth, atlasHeight);
    }

    public void Dispose() {
        _atlas.Dispose();
        GC.SuppressFinalize(this);
    }

    public record struct Range(char A, char B);
    public record struct Glyph(int U0, int V0, int U1, int V1, float XOffset, float YOffset, float XAdvance);
}