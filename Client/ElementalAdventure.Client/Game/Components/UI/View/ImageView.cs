using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Assets;
using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.Resources.HighLevel;
using ElementalAdventure.Client.Core.UI;
using ElementalAdventure.Client.Game.Components.Data;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Components.UI.Views;

public class ImageView : ViewBase {
    private readonly UserInterfaceShaderLayout.GlobalData[] _globalData;
    private readonly AssetManager _assetManager;
    private Vector2 _size;
    private AspectRatioType _aspectRatio;
    private AssetID _imageTextureAtlas, _imageTextureEntry;

    public Vector2 Size { get => _size; set { _size = value; InvalidateLayout(); } }
    public AspectRatioType AspectRatio { get => _aspectRatio; set { _aspectRatio = value; InvalidateLayout(); } }
    public AssetID ImageTextureAtlas { get => _imageTextureAtlas; set { _imageTextureAtlas = value; } }
    public AssetID ImageTextureEntry { get => _imageTextureEntry; set { _imageTextureEntry = value; } }

    public ImageView(AssetManager assetManager) {
        _assetManager = assetManager;
        _globalData = [new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f)), new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f)), new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f))];
        _aspectRatio = AspectRatioType.None;
        _imageTextureAtlas = AssetID.None;
        _imageTextureEntry = AssetID.None;
    }

    public override void Measure(Vector2 available) {
        _computedSize.X = (_size.X >= 0.0f && _size.X <= 1.0f) ? available.X * _size.X : _size.X;
        _computedSize.Y = (_size.Y >= 0.0f && _size.Y <= 1.0f) ? available.Y * _size.Y : _size.Y;

        if (_imageTextureAtlas != AssetID.None && _imageTextureEntry != AssetID.None && _aspectRatio != AspectRatioType.None) {
            TextureAtlas.Entry entry = _assetManager.Get<TextureAtlas>(_imageTextureAtlas).GetEntry(_imageTextureEntry);
            float target = entry.Width / (float)entry.Height;
            if (_aspectRatio == AspectRatioType.AdjustWidth) _computedSize.X = _computedSize.Y * target;
            else if (_aspectRatio == AspectRatioType.AdjustHeight) _computedSize.Y = _computedSize.X / target;
        }
    }

    public override void Render(IRenderer renderer) {
        if (_imageTextureAtlas == AssetID.None || _imageTextureEntry == AssetID.None)
            return;

        TextureAtlas atlas = _assetManager.Get<TextureAtlas>(_imageTextureAtlas);
        TextureAtlas.Entry entry = atlas.GetEntry(_imageTextureEntry);

        Span<byte> slot = renderer.AllocateInstance(this, 0, new AssetID("shader.userinterface"), _imageTextureAtlas, MemoryMarshal.Cast<UserInterfaceShaderLayout.GlobalData, byte>(_globalData.AsSpan()), Marshal.SizeOf<UserInterfaceShaderLayout.InstanceData>());
        UserInterfaceShaderLayout.InstanceData instance = new(_computedPosition, _computedSize, Vector3.One, 1, entry.Index, new Vector2i(entry.Width, entry.Height), entry.FrameCount, entry.FrameTime, 0, Vector4.Zero);
        MemoryMarshal.Write(slot, instance);
    }

    public enum AspectRatioType { None, AdjustWidth, AdjustHeight }
}