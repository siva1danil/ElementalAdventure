using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.WorldLogic.Component.Data;

public class HitboxDataComponent : IDataComponent {
    private readonly Box2 _box;

    public Box2 Box => _box;

    public HitboxDataComponent(Box2 box) {
        _box = box;
    }
}