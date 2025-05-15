using System.Diagnostics;

using ElementalAdventure.Client.Core.Resources.OpenGL;

using TrueTypeSharp;

namespace ElementalAdventure.Client.Core.Resources.Composed;

public class Font : IDisposable {
    private readonly Texture2D _atlas;
    private readonly Dictionary<char, Glyph> _glyphs;
    private readonly float _ascent, _descent;

    public int Id => _atlas.Id;
    public Dictionary<char, Glyph> Glyphs => _glyphs;
    public float Ascent => _ascent;
    public float Descent => _descent;

    public Font(byte[] data, Range[] ranges, int height, int padding, bool threshold = false) {
        if (ranges.Length == 0)
            throw new ArgumentException("TrueTypeFont must contain at least one range.");

        _glyphs = [];
        _ascent = 0;
        _descent = 0;

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
        Debug.WriteLine($"Font atlas size: {atlasWidth}x{atlasHeight}px, {atlasCols}x{atlasRows} cells of {count} glyphs, {maxWidth}x{maxHeight} max size");

        FontBitmap bitmap = new(atlasWidth, atlasHeight);
        for (int i = 0; i < ranges.Length; i++) {
            BakedChar[] baked = new BakedChar[ranges[i].B - ranges[i].A + 1];
            int result = font.BakeFontBitmap(height, ranges[i].A, baked, bitmap);
            if (result <= 0)
                throw new Exception($"Failed to bake font bitmap: {result}");
            for (int j = 0; j < baked.Length; j++) {
                char c = (char)(ranges[i].A + j);
                _glyphs[c] = new Glyph(baked[j].X0 / (float)atlasWidth, baked[j].Y0 / (float)atlasHeight,
                    baked[j].X1 / (float)atlasWidth, baked[j].Y1 / (float)atlasHeight, baked[j].XOffset / height,
                    baked[j].YOffset / height, baked[j].XAdvance / height, (baked[j].X1 - baked[j].X0) / (float)height, (baked[j].Y1 - baked[j].Y0) / (float)height);
                _ascent = Math.Max(_ascent, -baked[j].YOffset);
                _descent = Math.Max(_descent, baked[j].Y1 - baked[j].Y0 + baked[j].YOffset);
            }
        }

        byte[] rgba = new byte[atlasWidth * atlasHeight * 4];
        for (int i = 0; i < bitmap.Buffer.Length; i++) {
            rgba[i * 4] = 255;
            rgba[i * 4 + 1] = 255;
            rgba[i * 4 + 2] = 255;
            rgba[i * 4 + 3] = threshold ? (bitmap.Buffer[i] > 0 ? (byte)255 : (byte)0) : bitmap.Buffer[i];
        }

        _atlas = new Texture2D(rgba, atlasWidth, atlasHeight);
        _ascent /= height;
        _descent /= height;
    }

    public void Dispose() {
        _atlas.Dispose();
        GC.SuppressFinalize(this);
    }

    public readonly record struct Range(char A, char B);
    public readonly record struct Glyph(float U0, float V0, float U1, float V1, float XOffset, float YOffset, float XAdvance, float XSize, float YSize);
}