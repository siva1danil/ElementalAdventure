
using ElementalAdventure.Common.Assets;

namespace ElementalAdventure.Client.Core.Rendering;

public interface IUniformProvider {
    public void GetUniformData(AssetID shaderProgram, AssetID textureAtlas, Span<byte> buffer);
}
