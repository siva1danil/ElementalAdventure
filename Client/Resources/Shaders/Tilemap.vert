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

mat4 ortho(float l, float r, float b, float t, float n, float f) {
    float invWidth  = 1.0 / (r - l);
    float invHeight = 1.0 / (t - b);
    float invDepth  = 1.0 / (f - n);
    return mat4(
        vec4(2.0 * invWidth, 0.0,             0.0,              0.0),
        vec4(0.0,            2.0 * invHeight, 0.0,              0.0),
        vec4(0.0,            0.0,            -2.0 * invDepth,   0.0),
        vec4(-(r + l) * invWidth, -(t + b) * invHeight, -(f + n) * invDepth, 1.0)
    );
}

void main() {
    vec2 uvSize = vec2(uTileSize) / vec2(uTextureSize);
    int cols = uTextureSize.x / uTileSize.x;
    int rows = uTextureSize.y / uTileSize.y;
    int row = aInstances[gl_InstanceID].index / cols;
    int col = aInstances[gl_InstanceID].index % cols;
    vUV = vec2(col, row) * uvSize + vec2(aPosition.x, 1.0f - aPosition.y) * uvSize;
    gl_Position = ortho(-5.0, 5.0, -5.0, 5.0, -1.0, 1.0) * vec4(aInstances[gl_InstanceID].position + aPosition, 1.0f);
}