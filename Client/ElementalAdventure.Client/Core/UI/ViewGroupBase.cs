using System.Collections.ObjectModel;

using ElementalAdventure.Client.Core.Rendering;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Core.UI;

public abstract class ViewGroupBase : IViewGroup {
    protected readonly List<IView> _views = [];
    protected readonly Dictionary<IView, IViewGroup.ILayoutParams> _layoutParams = [];
    protected IViewGroup? _parent = null;

    protected bool _layoutDirty = false;
    protected Vector2 _computedSize = Vector2.Zero;
    protected Vector3 _computedPosition = Vector3.Zero;

    public IViewGroup? Parent { get => _parent; set => _parent = value; }

    public bool LayoutDirty { get => _layoutDirty; set => _layoutDirty = value; }
    public Vector2 ComputedSize { get => _computedSize; set => _computedSize = value; }
    public Vector3 ComputedPosition { get => _computedPosition; set => _computedPosition = value; }
    public ReadOnlyCollection<IView> Children => _views.AsReadOnly();

    public void InvalidateLayout() {
        if (_parent != null)
            _parent.InvalidateLayout();
        else
            _layoutDirty = true;
    }

    public void Add(IView view, IViewGroup.ILayoutParams layoutParams) {
        if (view.Parent != null)
            throw new ArgumentException($"View already has a parent: {view.Parent}.");
        view.Parent = this;
        _views.Add(view);
        _layoutParams[view] = layoutParams;

        InvalidateLayout();
    }

    public void Remove(IView view) {
        view.Parent = null;

        _views.Remove(view);
        _layoutParams.Remove(view);

        InvalidateLayout();
    }

    public void Clear() {
        foreach (IView view in _views)
            view.Parent = null;

        _views.Clear();
        _layoutParams.Clear();

        InvalidateLayout();
    }

    public abstract void Measure(Vector2 available);
    public abstract void Layout(float depth = 0.0f, float step = 0.0f);
    public abstract void Render(IRenderer renderer);
}