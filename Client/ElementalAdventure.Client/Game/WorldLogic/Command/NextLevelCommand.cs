using ElementalAdventure.Client.Game.SystemLogic;
using ElementalAdventure.Client.Game.WorldLogic.GameObject;
using ElementalAdventure.Common.Packets.Impl;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.WorldLogic.Command;

public class NextLevelCommand : ICommand {
    public NextLevelCommand() { }

    public void Execute(GameWorld world, ClientContext context) {
        _ = context.PacketClient.Connection?.SendAsync(new NextLevelPacket());
        _ = context.PacketClient.Connection?.SendAsync(new LoadWorldRequestPacket());
    }
}