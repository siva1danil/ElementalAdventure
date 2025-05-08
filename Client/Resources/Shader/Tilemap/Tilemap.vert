#version 330 core

layout (location = 0) in vec3 aGlobalPosition;
layout (location = 1) in vec3 aInstancePositionLast;
layout (location = 2) in vec3 aInstancePositionCurrent;
layout (location = 3) in int aInstanceIndex;
layout (location = 4) in ivec2 aInstanceFrameSize;
layout (location = 5) in int aInstanceFrameCount;
layout (location = 6) in int aInstanceFrameTime;

layout (std140) uniform Uniforms {
    mat4 uProjection;
    uvec2 uTimeMilliseconds;
    float uAlpha;
    ivec2 uTextureSize;
    ivec2 uCellSize;
    int uPadding;
};

out vec2 vUV;

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
    int index = aInstanceIndex + int(uTimeMilliseconds.y / uint(aInstanceFrameTime)) % aInstanceFrameCount;
    vec2 vertexUV = getVertexUV();
    vUV = getUV(uTextureSize, uCellSize, uPadding, index, aInstanceFrameSize, vertexUV);
    gl_Position = uProjection * vec4(aGlobalPosition + mix(aInstancePositionLast, aInstancePositionCurrent, uAlpha), 1.0f);
}