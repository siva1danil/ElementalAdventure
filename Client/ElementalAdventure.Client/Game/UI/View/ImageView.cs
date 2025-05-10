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
    private ImageSource _source;

    public ImageSource Source { get => _source; set { _source = value; } }

    public ImageView() {
        _globalData = [new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f))];
        _source = new ImageSource(AssetID.None, default);
    }

    public override void Measure(Vector2 available) {
        _computedSize.X = (_size.X >= 0.0f && _size.X <= 1.0f) ? available.X * _size.X : _size.X;
        _computedSize.Y = (_size.Y >= 0.0f && _size.Y <= 1.0f) ? available.Y * _size.Y : _size.Y;
    }

    public override void Render(IRenderer renderer) {
        if (_source.TextureAtlas == AssetID.None)
            return;
        Span<byte> slot = renderer.AllocateInstance(this, 0, new AssetID("shader.userinterface"), _source.TextureAtlas, MemoryMarshal.Cast<UserInterfaceShaderLayout.GlobalData, byte>(_globalData.AsSpan()), Marshal.SizeOf<UserInterfaceShaderLayout.InstanceData>());
        UserInterfaceShaderLayout.InstanceData instance = new(_computedPosition, _computedSize, Vector3.One, 1, _source.Texture.Index, new Vector2i(_source.Texture.Width, _source.Texture.Height), _source.Texture.FrameCount, _source.Texture.FrameTime);
        MemoryMarshal.Write(slot, instance);
    }

    public readonly record struct ImageSource(AssetID TextureAtlas, TextureAtlas.Entry Texture);
}