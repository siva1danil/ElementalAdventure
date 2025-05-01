namespace ElementalAdventure.Client.Core.Rendering;

public interface IUniformProvider<K> where K : notnull {
    public byte[] GetUniformData(K shaderProgram, K textureAtlas);
}