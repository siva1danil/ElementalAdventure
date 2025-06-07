using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.Resources.HighLevel;
using ElementalAdventure.Client.Core.UI;
using ElementalAdventure.Client.Game.Components.Data;
using ElementalAdventure.Common.Assets;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Components.UI.Views;

public class TextView : ViewBase {
    private readonly MsdfShaderLayout.GlobalData[] _globalData;
    private readonly AssetManager _assetManager;
    private AssetID _font;
    private string _text;
    private float _height;

    private string _cachedText = string.Empty;

    public AssetID Font { get => _font; set { _font = value; InvalidateLayout(); } }
    public string Text { get => _text; set { _text = value; InvalidateLayout(); } }
    public float Height { get => _height; set { _height = value; InvalidateLayout(); } }

    public TextView(AssetManager assetManager) {
        _assetManager = assetManager;
        _globalData = [new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f)), new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f)), new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f))];
        _font = AssetID.None;
        _text = string.Empty;
        _height = 1.0f;
    }

    public override void Measure(Vector2 available) {
        _computedSize.X = (_font == AssetID.None || string.IsNullOrEmpty(_text)) ? 0.0f : Measure(_text);
        _computedSize.Y = (_font == AssetID.None || string.IsNullOrEmpty(_text)) ? 0.0f : _height;
    }

    public override void Render(IRenderer renderer) {
        if (_font == AssetID.None || string.IsNullOrEmpty(_text))
            return;
        Span<byte> slot = renderer.AllocateInstance(this, 0, new AssetID("shader.msdf"), _font, MemoryMarshal.Cast<MsdfShaderLayout.GlobalData, byte>(_globalData.AsSpan()), Marshal.SizeOf<MsdfShaderLayout.InstanceData>() * _text.Length);
        if (_text != _cachedText) {
            BuildGeometry(_text, slot);
            _cachedText = _text;
        }
    }

    private float Measure(string text) {
        MsdfFont font = _assetManager.Get<MsdfFont>(_font);
        float x = 0.0f;
        for (int i = 0; i < text.Length; i++) {
            char c = text[i];
            bool ok = font.Glyphs.TryGetValue(c, out MsdfFont.Glyph glyph);
            if (!ok) font.Glyphs.TryGetValue(' ', out glyph);
            x += glyph.Advance * _height;
        }
        return x;
    }

    private void BuildGeometry(string text, Span<byte> slot) {
        MsdfFont font = _assetManager.Get<MsdfFont>(_font);
        float x = 0.0f;
        for (int i = 0; i < text.Length; i++) {
            int c = text[i];
            bool ok = font.Glyphs.TryGetValue(c, out MsdfFont.Glyph glyph);
            if (!ok) font.Glyphs.TryGetValue(' ', out glyph);

            MsdfShaderLayout.InstanceData instance = new MsdfShaderLayout.InstanceData(new Vector3(_computedPosition.X + glyph.XOffset * _height + x, _computedPosition.Y + (glyph.YOffset - font.Descent + 1.0f) * _height, _computedPosition.Z), new Vector2(glyph.XSize * _height, glyph.YSize * _height), new Vector4(glyph.U0, glyph.V0, glyph.U1, glyph.V1));
            MemoryMarshal.Write(slot[(i * Marshal.SizeOf<MsdfShaderLayout.InstanceData>())..], instance);

            x += glyph.Advance * _height;
        }
    }
}