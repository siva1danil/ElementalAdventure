#version 430 core

layout (location = 0) in vec3 aPosition;
layout (std430, binding = 0) buffer InstanceBuffer {
    struct {
        vec3 position;
        int index;
        int frameCount;
        int frameTime;
    } aInstances[];
};

uniform mat4 uProjection;
uniform uvec2 uTimeMilliseconds;
uniform ivec2 uTextureSize;
uniform ivec2 uTileSize;
uniform int uPadding;

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
    int index = aInstances[gl_InstanceID].index + int(uTimeMilliseconds.y) / aInstances[gl_InstanceID].frameTime % aInstances[gl_InstanceID].frameCount;
    vUV = getUV(uTextureSize, uTileSize, uPadding, index, aPosition.xy);
    gl_Position = uProjection * vec4(aInstances[gl_InstanceID].position + aPosition, 1.0f);
}