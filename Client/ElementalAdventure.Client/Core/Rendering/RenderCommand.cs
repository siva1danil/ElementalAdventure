namespace ElementalAdventure.Client.Core.Rendering;

public struct RenderCommand<K>(K shaderProgam, K textureAtlas, byte[] vertexData, byte[] instanceData) where K : notnull {
    public K ShaderProgram = shaderProgam;
    public K TextureAtlas = textureAtlas;
    public byte[] VertexData = vertexData;
    public byte[] InstanceData = instanceData;
}