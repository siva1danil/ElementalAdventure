using System.Runtime.InteropServices;

using ElementalAdventure.Common.Assets;

namespace ElementalAdventure.Server.World;

public class Generator {
    public static LayoutRoom[,] GenerateLayout(int seed, int spread, Dictionary<RoomType, int> types) {
        Random random = new Random(seed);

        List<RoomType> queue = [];
        foreach (KeyValuePair<RoomType, int> kvp in types)
            for (int i = 0; i < kvp.Value; i++)
                queue.Add(kvp.Key);
        random.Shuffle(CollectionsMarshal.AsSpan(queue));

        LayoutRoom[,] grid = new LayoutRoom[(int)Math.Ceiling(Math.Sqrt(queue.Count)) + spread, (int)Math.Ceiling(Math.Sqrt(queue.Count)) + spread];
        for (int y = 0; y < grid.GetLength(0); y++)
            for (int x = 0; x < grid.GetLength(1); x++)
                grid[y, x] = LayoutRoom.Empty;

        Stack<(int x, int y)> stack = new(queue.Count);
        List<(int x, int y)> valid = new(4);
        stack.Push((random.Next(grid.GetLength(1)), random.Next(grid.GetLength(0))));
        while (queue.Count > 0) {
            (int x, int y) pos = stack.Peek();
            (int x, int y) up = (pos.x, pos.y + 1), down = (pos.x, pos.y - 1), left = (pos.x - 1, pos.y), right = (pos.x + 1, pos.y);
            if (up.y >= 0 && up.y < grid.GetLength(0) && grid[up.y, up.x] == LayoutRoom.Empty) valid.Add(up);
            if (down.y >= 0 && down.y < grid.GetLength(0) && grid[down.y, down.x] == LayoutRoom.Empty) valid.Add(down);
            if (left.x >= 0 && left.x < grid.GetLength(1) && grid[left.y, left.x] == LayoutRoom.Empty) valid.Add(left);
            if (right.x >= 0 && right.x < grid.GetLength(1) && grid[right.y, right.x] == LayoutRoom.Empty) valid.Add(right);
            if (valid.Count == 0) {
                stack.Pop();
            } else {
                (int x, int y) next = valid[random.Next(valid.Count)];
                grid[next.y, next.x] = grid[pos.y, pos.x] != LayoutRoom.Empty
                    ? new LayoutRoom(queue[^1], next.y == pos.y + 1, next.x == pos.x - 1, next.y == pos.y - 1, next.x == pos.x + 1)
                    : new LayoutRoom(queue[^1], false, false, false, false);
                if (grid[pos.y, pos.x] != LayoutRoom.Empty)
                    grid[pos.y, pos.x] = new LayoutRoom(grid[pos.y, pos.x].Type, grid[pos.y, pos.x].DoorUp || (pos.y == next.y + 1), grid[pos.y, pos.x].DoorRight || (pos.x == next.x - 1), grid[pos.y, pos.x].DoorDown || (pos.y == next.y - 1), grid[pos.y, pos.x].DoorLeft || (pos.x == next.x + 1));
                queue.RemoveAt(queue.Count - 1);
                valid.Clear();
                stack.Push(next);
            }
        }

        return grid;
    }

    public static TileMask[,] GenerateTilemask(LayoutRoom[,] layout, IWorldType type) {
        int width = 1 + (type.RoomWidth + 1) * layout.GetLength(1) + 2;
        int height = 1 + (type.RoomHeight + 1) * layout.GetLength(0) + 2;
        TileMask[,] tilemask = new TileMask[height, width];

        // Zero-fill
        for (int y = 0; y < layout.GetLength(0); y++)
            for (int x = 0; x < layout.GetLength(1); x++)
                tilemask[y, x] = TileMask.None;

        // Fill floor
        for (int y = 0; y < layout.GetLength(0); y++) {
            for (int x = 0; x < layout.GetLength(1); x++) {
                if (layout[y, x] == LayoutRoom.Empty) continue;
                int originX = x * (type.RoomWidth + 1) + 1 + 1, originY = y * (type.RoomHeight + 1) + 1 + 1;
                for (int i = 0; i < type.RoomHeight; i++)
                    for (int j = 0; j < type.RoomWidth; j++)
                        tilemask[originY + i, originX + j] = TileMask.Floor;
            }
        }

        // Fill walls
        for (int y = 0; y < layout.GetLength(0); y++) {
            for (int x = 0; x < layout.GetLength(1); x++) {
                if (layout[y, x] == LayoutRoom.Empty) continue;
                int originX = x * (type.RoomWidth + 1) + 1 + 1, originY = y * (type.RoomHeight + 1) + 1 + 1;
                for (int wx = originX - 1; wx <= originX + type.RoomWidth; wx++) {
                    tilemask[originY - 1, wx] = TileMask.Wall;
                    tilemask[originY + type.RoomHeight, wx] = TileMask.Wall;
                }
                for (int wy = originY - 1; wy <= originY + type.RoomHeight; wy++) {
                    tilemask[wy, originX - 1] = TileMask.Wall;
                    tilemask[wy, originX + type.RoomWidth] = TileMask.Wall;
                }
            }
        }

        // Fill doors
        for (int y = 0; y < layout.GetLength(0); y++) {
            for (int x = 0; x < layout.GetLength(1); x++) {
                if (layout[y, x] == LayoutRoom.Empty) continue;
                int originX = x * (type.RoomWidth + 1) + 1 + 1, originY = y * (type.RoomHeight + 1) + 1 + 1;
                if (layout[y, x].DoorUp)
                    tilemask[originY - 1, originX + type.RoomWidth / 2] = TileMask.Door;
                if (layout[y, x].DoorDown)
                    tilemask[originY + type.RoomHeight, originX + type.RoomWidth / 2] = TileMask.Door;
                if (layout[y, x].DoorRight)
                    tilemask[originY + type.RoomHeight / 2, originX + type.RoomWidth] = TileMask.Door;
                if (layout[y, x].DoorLeft)
                    tilemask[originY + type.RoomHeight / 2, originX - 1] = TileMask.Door;
                if (type.RoomWidth % 2 == 0) {
                    if (layout[y, x].DoorUp)
                        tilemask[originY - 1, originX + (type.RoomWidth / 2) - 1] = TileMask.Door;
                    if (layout[y, x].DoorDown)
                        tilemask[originY + type.RoomHeight, originX + (type.RoomWidth / 2) - 1] = TileMask.Door;
                    if (layout[y, x].DoorRight)
                        tilemask[originY + (type.RoomHeight / 2) - 1, originX + type.RoomWidth] = TileMask.Door;
                    if (layout[y, x].DoorLeft)
                        tilemask[originY + (type.RoomHeight / 2) - 1, originX - 1] = TileMask.Door;
                }
            }
        }

        return tilemask;
    }

    public static AssetID[,,] GenerateTilemap(TileMask[,] tilemask, IWorldType type) {
        AssetID[,,] tilemap = new AssetID[type.LayerCount, tilemask.GetLength(0), tilemask.GetLength(1)];
        type.MapMaskToLayers(tilemap, tilemask);
        return tilemap;
    }

    public static WallBox[] GenerateWalls(TileMask[,] tilemask, IWorldType type) {
        List<WallBox> walls = [];
        for (int y = 0; y < tilemask.GetLength(0); y++) {
            for (int x = 0; x < tilemask.GetLength(1); x++) {
                bool startHorizontalWall = tilemask[y, x] == TileMask.Wall && (x == 0 || tilemask[y, x - 1] != TileMask.Wall) && (x == tilemask.GetLength(1) - 1 || tilemask[y, x + 1] == TileMask.Wall);
                bool startVerticalWall = tilemask[y, x] == TileMask.Wall && (y == 0 || tilemask[y - 1, x] != TileMask.Wall) && (y == tilemask.GetLength(0) - 1 || tilemask[y + 1, x] == TileMask.Wall);
                if (startHorizontalWall) {
                    int startX = x, endX = x;
                    while (endX < tilemask.GetLength(1) && tilemask[y, endX] == TileMask.Wall)
                        endX++;
                    walls.Add(new WallBox(x, y, endX - startX, 1));
                } else if (startVerticalWall) {
                    int startY = y, endY = y;
                    while (endY < tilemask.GetLength(0) && tilemask[endY, x] == TileMask.Wall)
                        endY++;
                    walls.Add(new WallBox(x, y, 1, endY - startY));
                }
            }
        }
        return [.. walls];
    }

    public enum TileMask : byte { None = 0, Floor = 1, Wall = 2, Door = 3 }
    public readonly record struct LayoutRoom(RoomType Type, bool DoorUp, bool DoorRight, bool DoorDown, bool DoorLeft) {
        public static LayoutRoom Empty => new(RoomType.None, false, false, false, false);
    }
    public readonly record struct WallBox(float X, float Y, float Width, float Height) { }
}