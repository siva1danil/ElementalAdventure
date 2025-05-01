namespace ElementalAdventure.Client.Core.Rendering;

public interface IRenderer<K> : IDisposable where K : notnull {
    public void Enqueue(IRenderable<K> renderable);
    public void Flush();
}