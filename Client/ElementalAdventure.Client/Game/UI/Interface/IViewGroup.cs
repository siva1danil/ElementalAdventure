namespace ElementalAdventure.Client.Game.UI.Interface;

public interface IViewGroup : IView {
    public void Layout();
    public void Add(IView view, ILayoutParams layoutParams);
    public void Remove(IView view);
    public void Clear();

    public interface ILayoutParams { }
}