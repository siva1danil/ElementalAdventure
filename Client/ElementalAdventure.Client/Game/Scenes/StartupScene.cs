using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.Resources.HighLevel;
using ElementalAdventure.Client.Core.UI;
using ElementalAdventure.Client.Game.Components.Data;
using ElementalAdventure.Client.Game.Components.UI.ViewGroups;
using ElementalAdventure.Client.Game.Components.UI.Views;
using ElementalAdventure.Client.Game.Components.Utils;
using ElementalAdventure.Client.Game.SystemLogic;
using ElementalAdventure.Common.Assets;

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
        ImageView background = new(_context.AssetManager) { Size = new Vector2(1.0f, 1.0f), AspectRatio = ImageView.AspectRatioType.AdjustWidth, ImageTextureAtlas = new AssetID("textureatlas.art"), ImageTextureEntry = new AssetID("background") };
        LinearLayout loadingLayout = new() { Orientation = LinearLayout.OrientationType.Horizontal, Gravity = LinearLayout.GravityType.Center };
        ImageView loading = new(_context.AssetManager) { Size = new Vector2(48f, 48f), AspectRatio = ImageView.AspectRatioType.None, ImageTextureAtlas = new AssetID("textureatlas.ui"), ImageTextureEntry = new AssetID("loading") };
        TextView text = new(_context.AssetManager) { Font = new AssetID("font.arial"), Text = "Connecting to server...", Height = 24f };
        layout.Add(background, new AbsoluteLayout.LayoutParams() { Position = new(0.5f, 0.5f), Anchor = new(0.5f, 0.5f) });
        layout.Add(loadingLayout, new AbsoluteLayout.LayoutParams() { Position = new(0.5f, 0.9f), Anchor = new(0.5f, 1.0f) });
        loadingLayout.Add(loading, new LinearLayout.LayoutParams { });
        loadingLayout.Add(text, new LinearLayout.LayoutParams { Margin = new(0.0f, 0.0f, 0.0f, 24.0f) });
        _ui.Push(layout);
    }

    public void Update(FrameEventArgs args) {
        if (_context.PacketClient.Awaiter == null)
            _context.PacketClient.Start();
    }

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

    public void MouseDown(MouseButtonEventArgs args, Vector2 position) { }

    public void MouseUp(MouseButtonEventArgs args, Vector2 position) { }

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
        } else if (shaderProgram == new AssetID("shader.msdf")) {
            MemoryMarshal.Write(buffer, new MsdfShaderLayout.UniformData(_uiCamera.GetViewMatrix()));
        } else {
            throw new NotImplementedException($"Shader program {shaderProgram} not supported in {nameof(StartupScene)}.");
        }
    }

    public void Dispose() {
        _renderer.Dispose();
        GC.SuppressFinalize(this);
    }
}