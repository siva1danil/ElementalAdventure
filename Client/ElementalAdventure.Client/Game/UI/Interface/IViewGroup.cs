namespace ElementalAdventure.Client.Game.UI.Interface;

public interface IViewGroup<T> : IView<T> where T : notnull {
    public void Layout();
    public void Add(IView<T> view, ILayoutParams layoutParams);
    public void Remove(IView<T> view);
    public void Clear();

    public interface ILayoutParams { }
}