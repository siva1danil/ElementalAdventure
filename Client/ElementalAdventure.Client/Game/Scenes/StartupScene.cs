using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Assets;
using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.Resources.Composed;
using ElementalAdventure.Client.Core.UI;
using ElementalAdventure.Client.Game.Data;
using ElementalAdventure.Client.Game.UI.View;
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
        _uiCamera = new Camera(_context.WindowSize / 2.0f, _context.WindowSize, _context.WindowSize, true);

        AbsoluteLayout layout = new();
        ImageView background = new(_context.AssetManager) { TextureAtlas = new AssetID("textureatlas.art"), TextureEntry = new AssetID("background"), Size = new Vector2(1.0f, 1.0f) };
        LinearLayout loadingLayout = new() { Orientation = LinearLayout.OrientationType.Horizontal, Gravity = LinearLayout.GravityType.Center };
        ImageView loading = new(_context.AssetManager) { TextureAtlas = new AssetID("textureatlas.ui"), TextureEntry = new AssetID("loading"), Size = new Vector2(48f, 48f) };
        TextView text = new(_context.AssetManager) { Font = new AssetID("font.arial"), Text = "Connecting to server...", Height = 32f, IgnoreDescent = true };
        layout.Add(background, new AbsoluteLayout.LayoutParams() { Position = new(0.0f, 0.0f), Anchor = new(0.0f, 0.0f) });
        layout.Add(loadingLayout, new AbsoluteLayout.LayoutParams() { Position = new(0.5f, 0.9f), Anchor = new(0.5f, 1.0f) });
        loadingLayout.Add(loading, new LinearLayout.LayoutParams { });
        loadingLayout.Add(text, new LinearLayout.LayoutParams { });
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
                UserInterfaceShaderLayout.UniformData data = new(_uiCamera.GetViewMatrix(), time, new(1, 1), new(1, 1), 0);
                if (_context.AssetManager.TryGet(textureAtlas, out TextureAtlas? atlas)) {
                    data.TextureSize = new Vector2i(atlas!.AtlasWidth, atlas!.AtlasHeight);
                    data.TextureCell = new Vector2i(atlas!.CellWidth, atlas!.CellHeight);
                    data.TexturePadding = atlas!.CellPadding;
                }
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