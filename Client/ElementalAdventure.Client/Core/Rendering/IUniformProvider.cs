namespace ElementalAdventure.Client.Core.Rendering;

public interface IUniformProvider<T> {
    public void GetUniformData(T shaderProgram, T textureAtlas, Span<byte> buffer);
}
