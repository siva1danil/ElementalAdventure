using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.UI;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.UI.ViewGroups;

public class AbsoluteLayout : ViewGroupBase {
    public AbsoluteLayout() { }

    public override void Measure() {
        Vector2 a = Vector2.Zero, b = Vector2.Zero;
        foreach (IView view in _views) {
            view.Measure();
            LayoutParams layoutParams = (LayoutParams)_layoutParams[view];
            a = Vector2.ComponentMin(a, layoutParams.Position - layoutParams.Anchor * view.Size);
            b = Vector2.ComponentMax(b, layoutParams.Position - layoutParams.Anchor * view.Size);
        }
        _size = b - a;
    }

    public override void Layout(float depth = 0.0f, float step = 0.0f) {
        foreach (IView view in _views) {
            LayoutParams layoutParams = (LayoutParams)_layoutParams[view];
            Vector2 position = layoutParams.Position - layoutParams.Anchor * view.Size;
            view.Position = new Vector3(position.X, position.Y, depth);

            if (view is IViewGroup group)
                group.Layout(depth + step, step);
        }
    }

    public override void Render(IRenderer renderer) {
        foreach (IView view in _views)
            view.Render(renderer);
    }

    public class LayoutParams : IViewGroup.ILayoutParams {
        private Vector2 _position, _anchor;

        public Vector2 Position { get => _position; set => _position = value; }
        public Vector2 Anchor { get => _anchor; set => _anchor = value; }
    }
}