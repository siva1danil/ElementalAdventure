using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Assets;
using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.Resources.Composed;
using ElementalAdventure.Client.Core.UI;
using ElementalAdventure.Client.Game.Data;
using ElementalAdventure.Client.Game.UI.ViewGroups;
using ElementalAdventure.Client.Game.UI.Views;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace ElementalAdventure.Client.Game.Scenes;

public class StartupScene : IScene, IUniformProvider {
    private readonly ClientContext _context;

    private readonly BatchedRenderer _renderer;
    private readonly UIManager _ui;
    private readonly Camera _uiCamera;

    public StartupScene(ClientContext context) {
        _context = context;

        _renderer = new BatchedRenderer(context.AssetManager, this);
        _ui = new UIManager(new(0.0f, 1.0f), _context.WindowSize);
        _uiCamera = new Camera(_context.WindowSize / 2.0f, _context.WindowSize, _context.WindowSize);

        AbsoluteLayout layout = new();
        ImageView background = new() { Size = new Vector2(1.0f, 1.0f), Source = new ImageView.ImageSource(new AssetID("textureatlas.art"), _context.AssetManager.Get<TextureAtlas>(new AssetID("textureatlas.art")).GetEntry(new AssetID("background"))) };
        layout.Add(background, new AbsoluteLayout.LayoutParams() { Position = new(0.0f, 0.0f), Anchor = new(0.0f, 0.0f) });
        _ui.Push(layout);
    }

    public void Update(FrameEventArgs args) { }

    public void Render(FrameEventArgs args) {
        _ui.Render(_renderer);
        _renderer.Commit();
        _renderer.Render();
    }

    public void Resize(ResizeEventArgs args) {
        _uiCamera.Center = new Vector2(args.Size.X, args.Size.Y) / 2.0f;
        _uiCamera.ScreenSize = new Vector2(args.Size.X, args.Size.Y);
        _uiCamera.TargetWorldSize = new Vector2(args.Size.X, args.Size.Y);
        _ui.Size = new Vector2(args.Size.X, args.Size.Y);
    }

    public void KeyDown(KeyboardKeyEventArgs args) { }

    public void KeyUp(KeyboardKeyEventArgs args) { }

    public void GetUniformData(AssetID shaderProgram, AssetID textureAtlas, Span<byte> buffer) {
        Vector2i time = new((int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >> 32), (int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() & 0xFFFFFFFF));
        if (shaderProgram == new AssetID("shader.userinterface")) {
            if (textureAtlas == AssetID.None) {
                UserInterfaceShaderLayout.UniformData data = new(_uiCamera.GetViewMatrix(), time);
                MemoryMarshal.Write(buffer, data);
            } else {
                TextureAtlas atlas = _context.AssetManager.Get<TextureAtlas>(textureAtlas);
                UserInterfaceShaderLayout.UniformData data = new(_uiCamera.GetViewMatrix(), time, new(atlas.AtlasWidth, atlas.AtlasHeight), new(atlas.CellWidth, atlas.CellHeight), atlas.CellPadding);
                MemoryMarshal.Write(buffer, data);
            }
        } else {
            throw new NotImplementedException($"Shader program {shaderProgram} not supported in {nameof(StartupScene)}.");
        }
    }

    public void Dispose() {
        _renderer.Dispose();
        GC.SuppressFinalize(this);
    }
}