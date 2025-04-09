#version 430 core

layout (location = 0) in vec3 aPosition;
layout (std430, binding = 0) buffer InstanceBuffer {
    struct {
        vec3 position;
        int index;
    } aInstances[];
};

uniform ivec2 uTextureSize;
uniform ivec2 uTileSize;

out vec2 vUV;

vec2 getUV(ivec2 textureSize, ivec2 tileSize, int index, vec2 position) {
    vec2 uvSize = vec2(tileSize) / vec2(textureSize);
    ivec2 colsrows = textureSize / tileSize;
    ivec2 colrow = ivec2(index % colsrows.x, index / colsrows.x);
    return vec2(colrow) * uvSize + vec2(position.x, 1.0f - position.y) * uvSize;
}

void main() {
    vUV = getUV(uTextureSize, uTileSize, aInstances[gl_InstanceID].index, aPosition.xy);
    gl_Position = vec4(aInstances[gl_InstanceID].position + aPosition, 1.0f);
}