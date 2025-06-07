using System.Diagnostics;

using ElementalAdventure.Client.Core.Resources.HighLevel;
using ElementalAdventure.Client.Core.Resources.OpenGL;
using ElementalAdventure.Client.Game.Components.Data;
using ElementalAdventure.Client.Game.PacketHandlers;
using ElementalAdventure.Client.Game.Scenes;
using ElementalAdventure.Client.Game.SystemLogic.Command;
using ElementalAdventure.Common.Assets;
using ElementalAdventure.Common.Logging;
using ElementalAdventure.Common.Networking;
using ElementalAdventure.Common.Packets;
using ElementalAdventure.Common.Packets.Impl;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ElementalAdventure.Client.Game.SystemLogic;

public class ClientWindow : GameWindow {
    private readonly ClientContext _context;
    private IScene? _scene;

    /* temp */
    private double _gpuFrameTimeMs = 0.0;
    private double _gpuFrameTimeAccum = 0.0;
    private int _gpuFrameCount = 0;
    private double _gpuReportTimer = 0.0;
    private readonly Stopwatch _gpuTimer = new();
    /* temp */

    public IScene? Scene => _scene;

    public ClientWindow(System.Net.IPEndPoint server, string root) : base(GameWindowSettings.Default, new NativeWindowSettings { Title = "Elemental Adventure", ClientSize = new(1280, 720), NumberOfSamples = 4 }) {
        Load += LoadHandler;
        Unload += UnloadHandler;
        UpdateFrame += UpdateFrameHandler;
        RenderFrame += RenderFrameHandler;
        Resize += ResizeHandler;
        KeyDown += KeyDownHandler;
        KeyUp += KeyUpHandler;

        PacketRegistry registry = new();

        _context = new ClientContext(
            new AssetLoader(Path.Combine(root, "Resources")),
            new AssetManager(),
            registry,
            new PacketClient(registry, server),
            ClientSize
        );

        _context.PacketClient.OnConnected += () => _context.PacketClient.Connection?.SendAsync(new HandshakeRequestPacket() { ClientVersion = 0 });
        _context.PacketClient.OnDisconnected += (ex) => _context.CommandQueue.Enqueue(new SetSceneCommand(new StartupScene(_context)));
        _context.PacketClient.OnPacketReceived += (packet) => _context.PacketRegistry.TryHandlePacket(_context.PacketClient.Connection!, packet);

        registry.RegisterPacket(PacketType.HandshakeResponse, HandshakeResponsePacket.Deserialize, new HandshakeResponsePacketHandler(_context, "guest", "00000000-0000-0000-0000-000000000000"));
        registry.RegisterPacket(PacketType.LoginResponse, LoginResponsePacket.Deserialize, new LoginResponsePacketHandler(_context));
        registry.RegisterPacket(PacketType.LoadWorldResponse, LoadWorldResponsePacket.Deserialize, new LoadWorldResponsePacketHandler(this, _context));
        registry.RegisterPacket(PacketType.SpawnEntity, SpawnEntityPacket.Deserialize, new SpawnEntityPacketHandler(_context));
    }

    private void LoadHandler() {
        GL.LoadBindings(new GLFWBindingsContext());

        try {
            _context.AssetManager.Add(new AssetID("shader.tilemap"), new ShaderProgram(_context.AssetLoader.LoadText("Shader/Tilemap/Tilemap.vert"), _context.AssetLoader.LoadText("Shader/Tilemap/Tilemap.frag")));
            _context.AssetManager.Add(new AssetID("shader.userinterface"), new ShaderProgram(_context.AssetLoader.LoadText("Shader/UserInterface/UserInterface.vert"), _context.AssetLoader.LoadText("Shader/UserInterface/UserInterface.frag")));
            _context.AssetManager.Add(new AssetID("shader.msdf"), new ShaderProgram(_context.AssetLoader.LoadText("Shader/Msdf/Msdf.vert"), _context.AssetLoader.LoadText("Shader/Msdf/Msdf.frag")));
            _context.AssetManager.Add(new AssetID("textureatlas.art"), new TextureAtlas(new Dictionary<AssetID, TextureAtlas.EntryDef> {
                { new AssetID("background"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Art/background.png")], 100) }
            }, 1));
            _context.AssetManager.Add(new AssetID("textureatlas.ui"), new TextureAtlas(new Dictionary<AssetID, TextureAtlas.EntryDef> {
                { new AssetID("loading"), new([
                    _context.AssetLoader.LoadBinary("TextureAtlas/UI/loading.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/UI/loading.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/UI/loading.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/UI/loading.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/UI/loading.4.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/UI/loading.5.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/UI/loading.6.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/UI/loading.7.png")
                ], 100) },
                { new AssetID("button_wasd_normal"), new([_context.AssetLoader.LoadBinary("TextureAtlas/UI/button_wasd_normal.png")], 100) },
                { new AssetID("button_q_normal"), new([_context.AssetLoader.LoadBinary("TextureAtlas/UI/button_q_normal.png")], 100) },
                { new AssetID("button_q_press"), new([_context.AssetLoader.LoadBinary("TextureAtlas/UI/button_q_press.png")], 100) },
                { new AssetID("button_e_normal"), new([_context.AssetLoader.LoadBinary("TextureAtlas/UI/button_e_normal.png")], 100) },
                { new AssetID("button_e_press"), new([_context.AssetLoader.LoadBinary("TextureAtlas/UI/button_e_press.png")], 100) },
                { new AssetID("button_tab_normal"), new([_context.AssetLoader.LoadBinary("TextureAtlas/UI/button_tab_normal.png")], 100) },
                { new AssetID("button_tab_press"), new([_context.AssetLoader.LoadBinary("TextureAtlas/UI/button_tab_press.png")], 100) },
                { new AssetID("icon_health"), new([_context.AssetLoader.LoadBinary("TextureAtlas/UI/icon_health.png")], 100) },
                { new AssetID("icon_depth"), new([_context.AssetLoader.LoadBinary("TextureAtlas/UI/icon_depth.png")], 100) }
            }, 1));
            _context.AssetManager.Add(new AssetID("textureatlas.dungeon"), new TextureAtlas(new Dictionary<AssetID, TextureAtlas.EntryDef> {
                { new AssetID("floor_1_full"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_1_full.png")], 100) },
                { new AssetID("floor_1_bottom"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_1_bottom.png")], 100) },
                { new AssetID("floor_1_top"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_1_top.png")], 100) },
                { new AssetID("floor_1_left"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_1_left.png")], 100) },
                { new AssetID("floor_1_right"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_1_right.png")], 100) },
                { new AssetID("floor_2_full"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_2_full.png")], 100) },
                { new AssetID("floor_2_bottom"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_2_bottom.png")], 100) },
                { new AssetID("floor_2_top"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_2_top.png")], 100) },
                { new AssetID("floor_2_left"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_2_left.png")], 100) },
                { new AssetID("floor_2_right"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_2_right.png")], 100) },
                { new AssetID("wall_bottom"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_bottom.png")], 100) },
                { new AssetID("wall_top"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_top.png")], 100) },
                { new AssetID("wall_left"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_left.png")], 100) },
                { new AssetID("wall_right"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_right.png")], 100) },
                { new AssetID("wall_lefttop"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_lefttop.png")], 100) },
                { new AssetID("wall_leftbottom"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_leftbottom.png")], 100) },
                { new AssetID("wall_righttop"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_righttop.png")], 100) },
                { new AssetID("wall_rightbottom"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_rightbottom.png")], 100) },
                { new AssetID("wall_bottom_connected"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_bottom_connected.png")], 100) },
                { new AssetID("wall_top_connected"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_top_connected.png")], 100) },
                { new AssetID("wall_side_connected"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_side_connected.png")], 100) },
                { new AssetID("wall_cross_lb_rb"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_cross_lb_rb.png")], 100) },
                { new AssetID("wall_cross_lt_lb"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_cross_lt_lb.png")], 100) },
                { new AssetID("wall_cross_lt_rt"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_cross_lt_rt.png")], 100) },
                { new AssetID("wall_cross_rt_rb"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_cross_rt_rb.png")], 100) },
                { new AssetID("wall_cross_lt_lb_rb"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_cross_lt_lb_rb.png")], 100) },
                { new AssetID("wall_cross_lt_rt_rb"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_cross_lt_rt_rb.png")], 100) },
                { new AssetID("wall_cross_lb_rt_rb"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_cross_lb_rt_rb.png")], 100) },
                { new AssetID("wall_cross_lt_lb_rt"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_cross_lt_lb_rt.png")], 100) },
                { new AssetID("wall_cross_lt_rt_lb_rb"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_cross_lt_rt_lb_rb.png")], 100) },
                { new AssetID("wall_doorway_horizontal_top"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_doorway_horizontal_top.png")], 100) },
                { new AssetID("wall_doorway_horizontal_bottom"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_doorway_horizontal_bottom.png")], 100) },
                { new AssetID("wall_doorway_vertical_top"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_doorway_vertical_top.png")], 100) },
                { new AssetID("wall_doorway_vertical_bottom"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_doorway_vertical_bottom.png")], 100) },
                { new AssetID("door_horizontal_closed_top"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/door_horizontal_closed_top.png")], 100) },
                { new AssetID("door_horizontal_closed_bottom"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/door_horizontal_closed_bottom.png")], 100) },
                { new AssetID("door_horizontal_open_top"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/door_horizontal_open_top.png")], 100) },
                { new AssetID("door_horizontal_open_bottom"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/door_horizontal_open_bottom.png")], 100) },
                { new AssetID("door_vertical_closed_top"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/door_vertical_closed_top.png")], 100) },
                { new AssetID("door_vertical_closed_bottom"), new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/door_vertical_closed_bottom.png")], 100) },
                { new AssetID("fountain_1_bottom"), new([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_bottom.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_bottom.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_bottom.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_bottom.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_bottom.4.png")
                ], 100) },
                { new AssetID("fountain_1_top"), new([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_top.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_top.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_top.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_top.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_top.4.png")
                ], 100) },
                { new AssetID("fountain_2_bottom"), new([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_bottom.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_bottom.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_bottom.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_bottom.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_bottom.4.png")
                ], 100) },
                { new AssetID("fountain_2_top"), new([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_top.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_top.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_top.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_top.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_top.4.png")
                ], 100) }
            }, 1));
            _context.AssetManager.Add(new AssetID("textureatlas.player"), new TextureAtlas(new Dictionary<AssetID, TextureAtlas.EntryDef> {
                { new AssetID("mage_idle_left"), new ([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_left.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_left.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_left.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_left.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_left.4.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_left.5.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_left.6.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_left.7.png")
                ], 100) },
                { new AssetID("mage_idle_right"), new ([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.4.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.5.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.6.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.7.png")
                ], 100) },
                { new AssetID("mage_walk_left"), new ([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_left.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_left.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_left.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_left.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_left.4.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_left.5.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_left.6.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_left.7.png")
                ], 100) },
                { new AssetID("mage_walk_right"), new ([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.4.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.5.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.6.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.7.png")
                ], 100) }
            }, 1));
            _context.AssetManager.Add(new AssetID("textureatlas.enemy"), new TextureAtlas(new Dictionary<AssetID, TextureAtlas.EntryDef> {
                { new AssetID("slime_walk_left"), new ([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Enemy/slime_walk_left.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Enemy/slime_walk_left.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Enemy/slime_walk_left.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Enemy/slime_walk_left.3.png")
                ], 100) },
                { new AssetID("slime_walk_right"), new ([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Enemy/slime_walk_right.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Enemy/slime_walk_right.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Enemy/slime_walk_right.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Enemy/slime_walk_right.3.png")
                ], 100) }
            }, 1));

            _context.AssetManager.Add(new AssetID("font.arial"), new MsdfFont(_context.AssetLoader.LoadBinary("Font/Arial/Arial.png"), _context.AssetLoader.LoadBinary("Font/Arial/Arial.json")));
            _context.AssetManager.Add(new AssetID("font.pixeloidsans"), new MsdfFont(_context.AssetLoader.LoadBinary("Font/PixeloidSans/PixeloidSans.png"), _context.AssetLoader.LoadBinary("Font/PixeloidSans/PixeloidSans.json")));

            _context.AssetManager.Add(new AssetID("floor_1_full"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("floor_1_full"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("floor_1_bottom"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("floor_1_bottom"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("floor_1_top"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("floor_1_top"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("floor_1_left"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("floor_1_left"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("floor_1_right"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("floor_1_right"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("floor_2_full"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("floor_2_full"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("floor_2_bottom"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("floor_2_bottom"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("floor_2_top"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("floor_2_top"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("floor_2_left"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("floor_2_left"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("floor_2_right"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("floor_2_right"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_bottom"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_bottom"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_top"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_top"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_left"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_left"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_right"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_right"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_lefttop"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_lefttop"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_leftbottom"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_leftbottom"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_righttop"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_righttop"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_rightbottom"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_rightbottom"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_bottom_connected"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_bottom_connected"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_top_connected"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_top_connected"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_side_connected"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_side_connected"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_cross_lb_rb"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_cross_lb_rb"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_cross_lb_rt"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_cross_lb_rt"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_cross_lt_lb"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_cross_lt_lb"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_cross_lt_rt"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_cross_lt_rt"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_cross_rt_rb"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_cross_rt_rb"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_cross_lt_lb_rb"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_cross_lt_lb_rb"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_cross_lt_rt_rb"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_cross_lt_rt_rb"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_cross_lb_rt_rb"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_cross_lb_rt_rb"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_cross_lt_lb_rt"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_cross_lt_lb_rt"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_cross_lt_rt_lb_rb"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_cross_lt_rt_lb_rb"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_doorway_horizontal_top"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_doorway_horizontal_top"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_doorway_horizontal_bottom"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_doorway_horizontal_bottom"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("wall_doorway_vertical_top"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_doorway_vertical_top"), 0, -0.5f + 5.0f / 32.0f));
            _context.AssetManager.Add(new AssetID("wall_doorway_vertical_bottom"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("wall_doorway_vertical_bottom"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("door_horizontal_closed_top"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("door_horizontal_closed_top"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("door_horizontal_closed_bottom"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("door_horizontal_closed_bottom"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("door_horizontal_open_top"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("door_horizontal_open_top"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("door_horizontal_open_bottom"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("door_horizontal_open_bottom"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("door_vertical_closed_top"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("door_vertical_closed_top"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("door_vertical_closed_bottom"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("door_vertical_closed_bottom"), 0, -0.5f));
            _context.AssetManager.Add(new AssetID("fountain_1_bottom"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("fountain_1_bottom"), 0, -0.5f + 7.0f / 32.0f));
            _context.AssetManager.Add(new AssetID("fountain_1_top"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("fountain_1_top"), -1, -0.5f - 1.0f + 7.0f / 32.0f));
            _context.AssetManager.Add(new AssetID("fountain_2_bottom"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("fountain_2_bottom"), 0, -0.5f + 7.0f / 32.0f));
            _context.AssetManager.Add(new AssetID("fountain_2_top"), new TileType(new AssetID("textureatlas.dungeon"), new AssetID("fountain_2_top"), -1, -0.5f - 1.0f + 7.0f / 32.0f));

            _context.AssetManager.Add(new AssetID("mage"), new PlayerType(new AssetID("textureatlas.player"), new AssetID("mage_idle_left"), new AssetID("mage_idle_right"), new AssetID("mage_walk_left"), new AssetID("mage_walk_right"), 0, -0.5f + 2.0f / 32.0f, 5.0f, 0.25f));
            _context.AssetManager.Add(new AssetID("slime"), new EnemyType(new AssetID("textureatlas.enemy"), new AssetID("slime_walk_left"), new AssetID("slime_walk_right"), new AssetID("slime_walk_left"), new AssetID("slime_walk_right"), 0, -0.5f + 2.0f / 32.0f, 2.5f, 0.125f));
            _context.AssetManager.Add(new AssetID("fireball"), new ProjectileType(new AssetID("textureatlas.projectile"), new AssetID("fireball"), 0, -0.5f, 1.0f, 0.25f, true, false));
        } catch (Exception e) {
            Logger.Error(e.Message);
            Close();
            return;
        }

        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Lequal);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.Multisample);

        _scene = new StartupScene(_context);
    }

    public void SetScene(IScene scene) {
        _scene?.Dispose();
        _scene = scene;
    }

    private void UnloadHandler() {
        _scene?.Dispose();
        _context.AssetManager.Dispose();
        _context.PacketClient.Stop().GetAwaiter().GetResult();
    }

    private void UpdateFrameHandler(FrameEventArgs args) {
        while (_context.CommandQueue.Count > 0) {
            IClientCommand command = _context.CommandQueue.Dequeue();
            Logger.Debug($"Executing command: {command.GetType().Name}");
            command.Execute(this, _scene, _context);
        }
        _scene?.Update(args);
    }

    private void RenderFrameHandler(FrameEventArgs args) {
        /* temp */
        _gpuTimer.Restart();

        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _scene?.Render(args);
        GL.Finish();

        _gpuTimer.Stop();
        _gpuFrameTimeAccum += _gpuTimer.Elapsed.TotalMilliseconds;
        _gpuFrameCount++;
        _gpuReportTimer += args.Time;
        if (_gpuReportTimer >= 1.0) {
            _gpuFrameTimeMs = _gpuFrameTimeAccum / _gpuFrameCount;
            Logger.Debug($"[GPU] Avg Frame Time: {_gpuFrameTimeMs * 1000:F3} µs over {_gpuFrameCount} frames");
            _gpuFrameTimeAccum = 0.0;
            _gpuFrameCount = 0;
            _gpuReportTimer = 0.0;
        }

        SwapBuffers();
        /* temp */
    }

    private void ResizeHandler(ResizeEventArgs e) {
        GL.Viewport(0, 0, e.Width, e.Height);
        _context.WindowSize = new(e.Width, e.Height);
        _scene?.Resize(e);
    }

    private void KeyDownHandler(KeyboardKeyEventArgs e) {
        if (!_context.PressedKeys.Contains(e.Key)) {
            _context.PressedKeys.Add(e.Key);
            _scene?.KeyDown(e);
        }
    }

    private void KeyUpHandler(KeyboardKeyEventArgs e) {
        if (_context.PressedKeys.Contains(e.Key)) {
            _context.PressedKeys.Remove(e.Key);
            _scene?.KeyUp(e);
        }
    }
}