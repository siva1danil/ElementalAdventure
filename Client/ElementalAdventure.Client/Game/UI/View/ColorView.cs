using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Assets;
using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.UI;
using ElementalAdventure.Client.Game.Data;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.UI.Views;

public class ColorView : ViewBase {
    private readonly UserInterfaceShaderLayout.GlobalData[] _globalData;
    private Vector3 _color;

    public Vector3 Color { get => _color; set { _color = value; } }

    public ColorView() {
        _globalData = [new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f))];
        _color = new(0.0f, 0.0f, 0.0f);
    }

    public override void Measure() { }

    public override void Render(IRenderer renderer) {
        Span<byte> slot = renderer.AllocateInstance(this, 0, new AssetID("shader.userinterface"), AssetID.None, MemoryMarshal.Cast<UserInterfaceShaderLayout.GlobalData, byte>(_globalData.AsSpan()), Marshal.SizeOf<UserInterfaceShaderLayout.InstanceData>());
        UserInterfaceShaderLayout.InstanceData instance = new(_position, _size, _color);
        MemoryMarshal.Write(slot, instance);
    }
}