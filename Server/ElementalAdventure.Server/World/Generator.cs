using System.Runtime.InteropServices;

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

    public static TileMask[,] GenerateTilemap(LayoutRoom[,] layout, WorldType type) {
        int width = 1 + (type.RoomWidth + 1) * layout.GetLength(1);
        int height = 1 + (type.RoomHeight + 1) * layout.GetLength(0);
        TileMask[,] tilemap = new TileMask[height, width];

        // Zero-fill
        for (int y = 0; y < layout.GetLength(0); y++)
            for (int x = 0; x < layout.GetLength(1); x++)
                tilemap[y, x] = TileMask.None;

        // Fill floor
        for (int y = 0; y < layout.GetLength(0); y++) {
            for (int x = 0; x < layout.GetLength(1); x++) {
                if (layout[y, x] == LayoutRoom.Empty) continue;
                int originX = x * (type.RoomWidth + 1) + 1, originY = y * (type.RoomHeight + 1) + 1;
                for (int i = 0; i < type.RoomHeight; i++)
                    for (int j = 0; j < type.RoomWidth; j++)
                        tilemap[originY + i, originX + j] = TileMask.Floor;
            }
        }

        // Fill walls
        for (int y = 0; y < layout.GetLength(0); y++) {
            for (int x = 0; x < layout.GetLength(1); x++) {
                if (layout[y, x] == LayoutRoom.Empty) continue;
                int originX = x * (type.RoomWidth + 1) + 1, originY = y * (type.RoomHeight + 1) + 1;
                for (int wx = originX - 1; wx <= originX + type.RoomWidth; wx++) {
                    tilemap[originY - 1, wx] = TileMask.Wall;
                    tilemap[originY + type.RoomHeight, wx] = TileMask.Wall;
                }
                for (int wy = originY - 1; wy <= originY + type.RoomHeight; wy++) {
                    tilemap[wy, originX - 1] = TileMask.Wall;
                    tilemap[wy, originX + type.RoomWidth] = TileMask.Wall;
                }
            }
        }

        // Fill doors
        for (int y = 0; y < layout.GetLength(0); y++) {
            for (int x = 0; x < layout.GetLength(1); x++) {
                if (layout[y, x] == LayoutRoom.Empty) continue;
                int originX = x * (type.RoomWidth + 1) + 1, originY = y * (type.RoomHeight + 1) + 1;
                if (type.RoomWidth % 2 == 0) {
                    if (layout[y, x].DoorUp) {
                        tilemap[originY - 1, originX + (type.RoomWidth / 2) - 1] = TileMask.DoorHorizontalLeft;
                        tilemap[originY - 1, originX + (type.RoomWidth / 2)] = TileMask.DoorHorizontalRight;
                    }
                    if (layout[y, x].DoorDown) {
                        tilemap[originY + type.RoomHeight, originX + (type.RoomWidth / 2) - 1] = TileMask.DoorHorizontalLeft;
                        tilemap[originY + type.RoomHeight, originX + (type.RoomWidth / 2)] = TileMask.DoorHorizontalRight;
                    }
                } else {
                    if (layout[y, x].DoorUp)
                        tilemap[originY - 1, originX + type.RoomWidth / 2] = TileMask.DoorHorizontal;
                    if (layout[y, x].DoorDown)
                        tilemap[originY + type.RoomHeight, originX + type.RoomWidth / 2] = TileMask.DoorHorizontal;
                }
                if (type.RoomHeight % 2 == 0) {
                    if (layout[y, x].DoorRight) {
                        tilemap[originY + (type.RoomHeight / 2) - 1, originX + type.RoomWidth] = TileMask.DoorVerticalTop;
                        tilemap[originY + (type.RoomHeight / 2), originX + type.RoomWidth] = TileMask.DoorVerticalBottom;
                    }
                    if (layout[y, x].DoorLeft) {
                        tilemap[originY + (type.RoomHeight / 2) - 1, originX - 1] = TileMask.DoorVerticalTop;
                        tilemap[originY + (type.RoomHeight / 2), originX - 1] = TileMask.DoorVerticalBottom;
                    }
                } else {
                    if (layout[y, x].DoorRight)
                        tilemap[originY + type.RoomHeight / 2, originX + type.RoomWidth] = TileMask.DoorVertical;
                    if (layout[y, x].DoorLeft)
                        tilemap[originY + type.RoomHeight / 2, originX - 1] = TileMask.DoorVertical;
                }
            }
        }

        return tilemap;
    }


    public enum TileMask : byte { None = 0, Floor = 1, Wall = 2, DoorHorizontal = 3, DoorVertical = 4, DoorHorizontalLeft = 5, DoorHorizontalRight = 6, DoorVerticalTop = 7, DoorVerticalBottom = 8 }
    public readonly record struct LayoutRoom(RoomType Type, bool DoorUp, bool DoorRight, bool DoorDown, bool DoorLeft) {
        public static LayoutRoom Empty => new(RoomType.None, false, false, false, false);
    }
}