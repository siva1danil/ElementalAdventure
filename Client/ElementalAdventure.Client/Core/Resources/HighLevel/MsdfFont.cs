using System.Text.Json;
using System.Text.Json.Serialization;

using ElementalAdventure.Client.Core.Resources.OpenGL;

using StbImageSharp;

namespace ElementalAdventure.Client.Core.Resources.HighLevel;

public class MsdfFont : IDisposable {
    private readonly Texture2D _atlas;
    private readonly Dictionary<int, Glyph> _glyphs;
    private readonly float _ascent, _descent;

    public int Id => _atlas.Id;
    public Dictionary<int, Glyph> Glyphs => _glyphs;
    public float Ascent => _ascent;
    public float Descent => _descent;

    public MsdfFont(byte[] atlas, byte[] glyphs) {
        MsdfJsonRoot json = JsonSerializer.Deserialize<MsdfJsonRoot>(glyphs) ?? throw new ArgumentException("Invalid MSDF font JSON: deserialization failed.");
        if (json.Atlas == null || json.Glyphs == null)
            throw new ArgumentException("Invalid MSDF font JSON: missing atlas or glyphs.");
        ImageResult image = ImageResult.FromMemory(atlas, ColorComponents.RedGreenBlueAlpha);

        _atlas = new(image.Data, image.Width, image.Height, true);
        _glyphs = new(json.Glyphs.Length);
        _ascent = json.Metrics?.Ascender ?? throw new ArgumentException("Invalid MSDF font JSON: missing metrics.");
        _descent = json.Metrics?.Descender ?? throw new ArgumentException("Invalid MSDF font JSON: missing metrics.");

        foreach (MsdfJsonGlyph glyph in json.Glyphs) {
            if (glyph.PlaneBounds != null && glyph.AtlasBounds != null) {
                MsdfJsonBounds a = glyph.AtlasBounds;
                MsdfJsonBounds p = glyph.PlaneBounds;

                float u0 = a.Left / json.Atlas.Width;
                float v0 = a.Bottom / json.Atlas.Height;
                float u1 = a.Right / json.Atlas.Width;
                float v1 = a.Top / json.Atlas.Height;

                float xOffset = p.Left;
                float yOffset = p.Bottom;
                float xSize = p.Right - p.Left;
                float ySize = p.Top - p.Bottom;

                _glyphs[glyph.Unicode] = new Glyph(u0, v0, u1, v1, xOffset, yOffset, xSize, ySize, glyph.Advance);
            } else {
                _glyphs[glyph.Unicode] = new Glyph(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, glyph.Advance);
            }
        }
    }

    public void Dispose() {
        _atlas.Dispose();
        GC.SuppressFinalize(this);
    }

    public readonly record struct Glyph(float U0, float V0, float U1, float V1, float XOffset, float YOffset, float XSize, float YSize, float Advance);

    private class MsdfJsonRoot {
        [JsonPropertyName("atlas")] public MsdfJsonAtlas? Atlas { get; set; }
        [JsonPropertyName("metrics")] public MsdfJsonMetrics? Metrics { get; set; }
        [JsonPropertyName("glyphs")] public MsdfJsonGlyph[]? Glyphs { get; set; }
        [JsonPropertyName("kerning")] public MsdfJsonKern[]? Kernings { get; set; }
    }

    private class MsdfJsonAtlas {
        [JsonPropertyName("type")] public string? Type { get; set; }
        [JsonPropertyName("distanceRange")] public float DistanceRange { get; set; }
        [JsonPropertyName("distanceRangeMiddle")] public float DistanceRangeMiddle { get; set; }
        [JsonPropertyName("size")] public float Size { get; set; }
        [JsonPropertyName("width")] public int Width { get; set; }
        [JsonPropertyName("height")] public int Height { get; set; }
    }

    private class MsdfJsonMetrics {
        [JsonPropertyName("emSize")] public float EmSize { get; set; }
        [JsonPropertyName("lineHeight")] public float LineHeight { get; set; }
        [JsonPropertyName("ascender")] public float Ascender { get; set; }
        [JsonPropertyName("descender")] public float Descender { get; set; }
        [JsonPropertyName("underlineY")] public float UnderlineY { get; set; }
        [JsonPropertyName("underlineThickness")] public float UnderlineThickness { get; set; }
    }

    private class MsdfJsonGlyph {
        [JsonPropertyName("unicode")] public int Unicode { get; set; }
        [JsonPropertyName("advance")] public float Advance { get; set; }
        [JsonPropertyName("planeBounds")] public MsdfJsonBounds? PlaneBounds { get; set; }
        [JsonPropertyName("atlasBounds")] public MsdfJsonBounds? AtlasBounds { get; set; }
    }

    private class MsdfJsonKern {
        [JsonPropertyName("unicode1")] public int Unicode1 { get; set; }
        [JsonPropertyName("unicode2")] public int Unicode2 { get; set; }
        [JsonPropertyName("advance")] public float Advance { get; set; }
    }

    private class MsdfJsonBounds {
        [JsonPropertyName("left")] public float Left { get; set; }
        [JsonPropertyName("bottom")] public float Bottom { get; set; }
        [JsonPropertyName("right")] public float Right { get; set; }
        [JsonPropertyName("top")] public float Top { get; set; }
    }
}