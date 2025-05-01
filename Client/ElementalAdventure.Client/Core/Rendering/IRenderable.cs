namespace ElementalAdventure.Client.Core.Rendering;

public interface IRenderable<K> where K : notnull {
    public RenderCommand<K>[] Render();
}