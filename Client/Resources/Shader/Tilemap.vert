#version 330 core

layout (location = 0) in vec3 aGlobalPosition;
layout (location = 1) in vec3 aInstancePositionLast;
layout (location = 2) in vec3 aInstancePositionCurrent;
layout (location = 3) in int aInstanceIndex;
layout (location = 4) in int aInstanceFrameCount;
layout (location = 5) in int aInstanceFrameTime;

layout(std140) uniform Uniforms {
    mat4 uProjection;
    uvec2 uTimeMilliseconds;
    float uAlpha;
    ivec2 uTextureSize;
    ivec2 uTileSize;
    int uPadding;
};

out vec2 vUV;

vec2 getUV(ivec2 textureSize, ivec2 tileSize, int padding, int index, vec2 position) {
    ivec2 paddedTileSize = tileSize + ivec2(padding) * 2;

    vec2 uvSize = vec2(paddedTileSize) / vec2(textureSize);
    ivec2 colsrows = textureSize / paddedTileSize;
    ivec2 colrow = ivec2(index % colsrows.x, index / colsrows.x);

    vec2 uvOffset = vec2(colrow * paddedTileSize + ivec2(padding));
    vec2 pixelPos = vec2(position.x * tileSize.x, (1.0f - position.y) * tileSize.y);

    return (uvOffset + pixelPos) / vec2(textureSize);
}

void main() {
    int index = aInstanceIndex + int(uTimeMilliseconds.y) / aInstanceFrameTime % aInstanceFrameCount;
    vUV = getUV(uTextureSize, uTileSize, uPadding, index, aGlobalPosition.xy);
    gl_Position = uProjection * vec4(aGlobalPosition + mix(aInstancePositionLast, aInstancePositionCurrent, uAlpha), 1.0f);
}