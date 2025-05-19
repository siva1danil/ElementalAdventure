using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Assets;
using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.UI;
using ElementalAdventure.Client.Game.Components.Data;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Components.UI.Views;

public class ColorView : ViewBase {
    private readonly UserInterfaceShaderLayout.GlobalData[] _globalData;
    private Vector2 _size;
    private Vector3 _color;

    public Vector2 Size { get => _size; set { _size = value; } }
    public Vector3 Color { get => _color; set { _color = value; } }

    public ColorView() {
        _globalData = [new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f)), new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f)), new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f))];
        _color = new(0.0f, 0.0f, 0.0f);
    }

    public override void Measure(Vector2 available) {
        _computedSize.X = (_size.X >= 0.0f && _size.X <= 1.0f) ? available.X * _size.X : _size.X;
        _computedSize.Y = (_size.Y >= 0.0f && _size.Y <= 1.0f) ? available.Y * _size.Y : _size.Y;
    }

    public override void Render(IRenderer renderer) {
        Span<byte> slot = renderer.AllocateInstance(this, 0, new AssetID("shader.userinterface"), AssetID.None, MemoryMarshal.Cast<UserInterfaceShaderLayout.GlobalData, byte>(_globalData.AsSpan()), Marshal.SizeOf<UserInterfaceShaderLayout.InstanceData>());
        UserInterfaceShaderLayout.InstanceData instance = new(_computedPosition, _computedSize, _color);
        MemoryMarshal.Write(slot, instance);
    }
}