using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Assets;
using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.Resources.Composed;
using ElementalAdventure.Client.Core.UI;
using ElementalAdventure.Client.Game.Data;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.UI.Views;

public class ImageView : ViewBase {
    private readonly UserInterfaceShaderLayout.GlobalData[] _globalData;
    private readonly AssetManager _assetManager;
    private AssetID _textureAtlas, _textureEntry;

    public AssetID TextureAtlas { get => _textureAtlas; set { _textureAtlas = value; } }
    public AssetID TextureEntry { get => _textureEntry; set { _textureEntry = value; } }

    public ImageView(AssetManager assetManager) {
        _assetManager = assetManager;
        _globalData = [new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f))];
        _textureAtlas = AssetID.None;
        _textureEntry = AssetID.None;
    }

    public override void Measure(Vector2 available) {
        _computedSize.X = (_size.X >= 0.0f && _size.X <= 1.0f) ? available.X * _size.X : _size.X;
        _computedSize.Y = (_size.Y >= 0.0f && _size.Y <= 1.0f) ? available.Y * _size.Y : _size.Y;
    }

    public override void Render(IRenderer renderer) {
        if (_textureAtlas == AssetID.None || _textureEntry == AssetID.None)
            return;

        TextureAtlas atlas = _assetManager.Get<TextureAtlas>(_textureAtlas);
        TextureAtlas.Entry entry = atlas.GetEntry(_textureEntry);

        Span<byte> slot = renderer.AllocateInstance(this, 0, new AssetID("shader.userinterface"), _textureAtlas, MemoryMarshal.Cast<UserInterfaceShaderLayout.GlobalData, byte>(_globalData.AsSpan()), Marshal.SizeOf<UserInterfaceShaderLayout.InstanceData>());
        UserInterfaceShaderLayout.InstanceData instance = new(_computedPosition, _computedSize, Vector3.One, 1, entry.Index, new Vector2i(entry.Width, entry.Height), entry.FrameCount, entry.FrameTime, 0, Vector4.Zero);
        MemoryMarshal.Write(slot, instance);
    }
}