using OpenTK.Mathematics;

public static class Box2Extensions {
    public static bool Intersects(this Box2 a, Box2 b) => a.Max.X > b.Min.X && a.Min.X < b.Max.X && a.Max.Y > b.Min.Y && a.Min.Y < b.Max.Y;
}