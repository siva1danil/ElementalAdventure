using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Assets;
using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.Resources.Composed;
using ElementalAdventure.Client.Core.UI;
using ElementalAdventure.Client.Game.Data;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.UI.View;

public class TextView : ViewBase {
    private readonly UserInterfaceShaderLayout.GlobalData[] _globalData;
    private readonly AssetManager _assetManager;
    private AssetID _font;
    private string _text;
    private float _height;

    public AssetID Font { get => _font; set { _font = value; } }
    public string Text { get => _text; set { _text = value; } }
    public float Height { get => _height; set { _height = value; } }

    public TextView(AssetManager assetManager) {
        _assetManager = assetManager;
        _globalData = [new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f)), new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f)), new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f))];
        _font = AssetID.None;
        _text = string.Empty;
        _height = 1.0f;
    }

    public override void Measure(Vector2 available) {
        _computedSize.X = (_font == AssetID.None || string.IsNullOrEmpty(_text)) ? 0.0f : Measure(_text);
        _computedSize.Y = _height;
    }

    public override void Render(IRenderer renderer) {
        if (_font == AssetID.None || string.IsNullOrEmpty(_text))
            return;
        Span<byte> slot = renderer.AllocateInstance(this, 0, new AssetID("shader.userinterface"), _font, MemoryMarshal.Cast<UserInterfaceShaderLayout.GlobalData, byte>(_globalData.AsSpan()), Marshal.SizeOf<UserInterfaceShaderLayout.InstanceData>() * _text.Length);
        BuildGeometry(_text, slot);
    }

    private float Measure(string text) {
        Font font = _assetManager.Get<Font>(_font);
        float x = 0.0f;
        for (int i = 0; i < text.Length; i++) {
            char c = text[i];
            bool ok = font.Glyphs.TryGetValue(c, out Font.Glyph glyph);
            if (!ok) font.Glyphs.TryGetValue(' ', out glyph);
            x += glyph.XAdvance * _height;
        }
        return x;
    }

    private void BuildGeometry(string text, Span<byte> slot) {
        Font font = _assetManager.Get<Font>(_font);
        float x = 0.0f;
        for (int i = 0; i < text.Length; i++) {
            char c = text[i];
            bool ok = font.Glyphs.TryGetValue(c, out Font.Glyph glyph);
            if (!ok) font.Glyphs.TryGetValue(' ', out glyph);

            UserInterfaceShaderLayout.InstanceData instance = new UserInterfaceShaderLayout.InstanceData(new Vector3(_computedPosition.X + glyph.XOffset * _height + x, _computedPosition.Y + (glyph.YOffset + font.Ascent) * _height, _computedPosition.Z), new Vector2(glyph.XSize * _height, glyph.YSize * _height), new Vector3(1, 1, 1), 1, 0, new Vector2i(0, 0), 0, 1, 1, new Vector4(glyph.U0, glyph.V0, glyph.U1, glyph.V1));
            MemoryMarshal.Write(slot[(i * Marshal.SizeOf<UserInterfaceShaderLayout.InstanceData>())..], instance);

            x += glyph.XAdvance * _height;
        }
    }
}