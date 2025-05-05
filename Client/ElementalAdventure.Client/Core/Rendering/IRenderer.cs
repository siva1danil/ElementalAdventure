namespace ElementalAdventure.Client.Core.Rendering;

public interface IRenderer<T> : IDisposable where T : notnull {
    public Span<byte> AllocateInstance(object ownerIdentity, int ownerIndex, T shaderProgram, T textureAtlas, Span<byte> vertexData, int instanceSize);
}