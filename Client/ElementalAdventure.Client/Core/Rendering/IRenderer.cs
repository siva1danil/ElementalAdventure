namespace ElementalAdventure.Client.Core.Rendering;

public interface IRenderer {
    public Span<byte> AllocateInstance(object ownerIdentity, int ownerIndex, string shaderProgram, string textureAtlas, Span<byte> vertexData, int instanceSize);
}