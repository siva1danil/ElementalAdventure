#version 330 core

layout (location = 0) in vec3 aGlobalPosition;
layout (location = 1) in vec3 aInstancePosition;
layout (location = 2) in vec2 aInstanceScale;
layout (location = 3) in vec3 aInstanceColor;
layout (location = 4) in int aInstanceHasTexture;
layout (location = 5) in int aInstanceFrameIndex;
layout (location = 6) in ivec2 aInstanceFrameSize;
layout (location = 7) in int aInstanceFrameCount;
layout (location = 8) in int aInstanceFrameTime;

layout (std140) uniform Uniforms {
    mat4 uProjection;
    uvec2 uTimeMilliseconds;
    ivec2 uTextureSize;
    ivec2 uTextureCell;
    int uTexturePadding;
};

out vec3 vColor;
out vec2 vUV;
flat out int vHasTexture;

vec2 getVertexUV() {
    const vec2 lut[6] = vec2[6](vec2(0, 1), vec2(1, 1), vec2(0, 0), vec2(1, 1), vec2(0, 0), vec2(1, 0));
    return lut[gl_VertexID % 6];
}

vec2 getUV(ivec2 textureSize, ivec2 cellSize, int padding, int index, ivec2 size, vec2 vertexUV) {
    ivec2 paddedTileSize = cellSize + ivec2(padding) * 2;

    vec2 uvSize = vec2(paddedTileSize) / vec2(textureSize);
    ivec2 colsrows = textureSize / paddedTileSize;
    ivec2 colrow = ivec2(index % colsrows.x, index / colsrows.x);

    vec2 uvOffset = vec2(colrow * paddedTileSize + ivec2(padding));
    vec2 pixelPos = vec2(vertexUV.x * size.x, vertexUV.y * size.y);

    return (uvOffset + pixelPos) / vec2(textureSize);
}

void main() {
    int index = aInstanceFrameIndex + int(uTimeMilliseconds.y / uint(aInstanceFrameTime)) % aInstanceFrameCount;
    vUV = getUV(uTextureSize, uTextureCell, uTexturePadding, index, aInstanceFrameSize, getVertexUV());
    vColor = aInstanceColor;
    vHasTexture = aInstanceHasTexture;
    gl_Position = uProjection * vec4(vec3(aGlobalPosition.xy * aInstanceScale, aGlobalPosition.z) + aInstancePosition, 1.0f);
}