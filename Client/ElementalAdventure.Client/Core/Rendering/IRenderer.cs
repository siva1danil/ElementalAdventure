using ElementalAdventure.Common.Assets;

namespace ElementalAdventure.Client.Core.Rendering;

public interface IRenderer : IDisposable {
    public Span<byte> AllocateInstance(object ownerIdentity, int ownerIndex, AssetID shaderProgram, AssetID textureAtlas, Span<byte> vertexData, int instanceSize);
}