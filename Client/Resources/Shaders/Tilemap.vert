#version 430 core

layout (location = 0) in vec3 aPosition;
layout (std430, binding = 0) buffer InstanceBuffer {
    struct {
        vec3 position;
        vec4 uv;
    } aInstances[];
};

uniform vec2 uTextureSize;
uniform vec2 uTileSize;

out vec2 vUV;

void main() {
    vUV = aInstances[gl_InstanceID].uv.xy + aPosition.xy * aInstances[gl_InstanceID].uv.zw;
    gl_Position = vec4(aInstances[gl_InstanceID].position + aPosition, 1.0f);
}