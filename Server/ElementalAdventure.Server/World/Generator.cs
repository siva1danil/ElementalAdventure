using System.Runtime.InteropServices;

namespace ElementalAdventure.Server.World;

public class Generator {
    public static RoomType?[,] GenerateLayout(int seed, int spread, Dictionary<RoomType, int> types) {
        Random random = new Random(seed);

        List<RoomType> queue = [];
        foreach (KeyValuePair<RoomType, int> kvp in types)
            for (int i = 0; i < kvp.Value; i++)
                queue.Add(kvp.Key);
        random.Shuffle(CollectionsMarshal.AsSpan(queue));
        RoomType?[,] grid = new RoomType?[(int)Math.Ceiling(Math.Sqrt(queue.Count)) + spread, (int)Math.Ceiling(Math.Sqrt(queue.Count)) + spread];

        Stack<(int x, int y)> stack = new(queue.Count);
        List<(int x, int y)> valid = new(4);
        stack.Push((random.Next(grid.GetLength(0)), random.Next(grid.GetLength(1) / 2)));
        while (queue.Count > 0) {
            (int x, int y) pos = stack.Peek();
            (int x, int y) up = (pos.x, pos.y + 1), down = (pos.x, pos.y - 1), left = (pos.x - 1, pos.y), right = (pos.x + 1, pos.y);
            if (up.y >= 0 && up.y < grid.GetLength(1) && grid[up.x, up.y] == null) valid.Add(up);
            if (down.y >= 0 && down.y < grid.GetLength(1) && grid[down.x, down.y] == null) valid.Add(down);
            if (left.x >= 0 && left.x < grid.GetLength(0) && grid[left.x, left.y] == null) valid.Add(left);
            if (right.x >= 0 && right.x < grid.GetLength(0) && grid[right.x, right.y] == null) valid.Add(right);
            if (valid.Count == 0) {
                stack.Pop();
            } else {
                (int x, int y) next = valid[random.Next(valid.Count)];
                grid[next.x, next.y] = queue[queue.Count - 1];
                queue.RemoveAt(queue.Count - 1);
                valid.Clear();
                stack.Push(next);
            }
        }

        return grid;
    }
}