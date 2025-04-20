using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Utils;

public static class VectorExtensions {
    public static void NormalizeOrZero(this Vector2 vector) {
        if (vector.LengthSquared > 0.0f) vector.Normalize();
    }

    public static Vector2 NormalizedOrZero(this Vector2 vector) {
        return vector.LengthSquared > 0.0f ? vector.Normalized() : Vector2.Zero;
    }
}