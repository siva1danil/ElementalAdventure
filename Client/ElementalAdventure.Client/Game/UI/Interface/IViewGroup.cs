using System.Collections.ObjectModel;

namespace ElementalAdventure.Client.Game.UI.Interface;

public interface IViewGroup : IView {
    public bool LayoutDirty { get; set; }
    public ReadOnlyCollection<IView> Children { get; }

    public void Add(IView view, ILayoutParams layoutParams);
    public void Remove(IView view);
    public void Clear();

    public void Layout(float depth = 0.0f, float step = 0.0f);

    public interface ILayoutParams { }
}