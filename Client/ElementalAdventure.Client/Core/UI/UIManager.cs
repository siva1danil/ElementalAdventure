using ElementalAdventure.Client.Core.Rendering;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Core.UI;

public class UIManager {
    private readonly Stack<IViewGroup> _stack;
    private readonly Vector2 _depthRange;
    private Vector2 _size;

    public Vector2 Size { get => _size; set { _size = value; foreach (IViewGroup group in _stack) group.InvalidateLayout(); } }

    public UIManager(Vector2 depthRange, Vector2 size) {
        _stack = [];
        _depthRange = depthRange;
        _size = size;
    }

    public void Push(IViewGroup viewGroup) {
        _stack.Push(viewGroup);
    }

    public void Pop() {
        if (_stack.Count != 0)
            _stack.Pop();
    }

    public void Render(IRenderer renderer) {
        if (_stack.Count == 0)
            return;

        IViewGroup group = _stack.Peek();
        if (group.LayoutDirty) {
            group.Measure(_size);
            group.Layout(_depthRange.X, (_depthRange.Y - _depthRange.X) / ComputeDepth(group));
            group.LayoutDirty = false;
        }
        group.Render(renderer);
    }

    private static int ComputeDepth(IViewGroup root) {
        int max = 0;
        Stack<(IView node, int depth)> stack = new();
        stack.Push((root, 1));

        while (stack.Count > 0) {
            (IView node, int depth) = stack.Pop();
            max = Math.Max(max, depth);
            if (node is IViewGroup group)
                foreach (IView child in group.Children)
                    stack.Push((child, depth + 1));
        }

        return max;
    }
}