using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Game.Data;
using ElementalAdventure.Client.Game.UI.Interface;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.UI.View;

public class ColorView : IView<string> {
    private Vector2 _size = Vector2.One;
    private Vector3 _calculatedPosition = Vector3.Zero;
    private Vector3 _color = Vector3.One;

    private readonly UserInterfaceShaderLayout.GlobalData[] _globalData;

    public Vector2 Size { get => _size; set => _size = value; }
    public Vector3 CalculatedPosition { get => _calculatedPosition; set => _calculatedPosition = value; }
    public Vector3 Color { get => _color; set => _color = value; }

    public ColorView() {
        _globalData = [new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f))];
    }

    public void Measure() {
        // No measurement needed
    }

    public void Render(IRenderer<string> renderer) {
        Span<byte> slot = renderer.AllocateInstance(this, 0, "shader.userinterface", "textureatlas.dungeon", MemoryMarshal.Cast<UserInterfaceShaderLayout.GlobalData, byte>(_globalData.AsSpan()), Marshal.SizeOf<UserInterfaceShaderLayout.InstanceData>());
        UserInterfaceShaderLayout.InstanceData instance = new(_calculatedPosition, _size, _color);
        MemoryMarshal.Write(slot, instance);
    }
}