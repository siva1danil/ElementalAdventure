#version 330 core

layout (location = 0) in vec3 aGlobalPosition;
layout (location = 1) in vec3 aInstancePosition;
layout (location = 2) in vec2 aInstanceScale;
layout (location = 3) in vec4 aInstanceUV;

layout (std140) uniform Uniforms {
    mat4 uProjection;
};

out vec2 vUV;

vec2 getVertexUV() {
    const vec2 lut[6] = vec2[6](vec2(0, 1), vec2(1, 1), vec2(0, 0), vec2(1, 1), vec2(0, 0), vec2(1, 0));
    return lut[gl_VertexID % 6];
}

void main() {
    vUV = aInstanceUV.xy + (aInstanceUV.zw - aInstanceUV.xy) * getVertexUV();
    gl_Position = uProjection * vec4(vec3(aGlobalPosition.xy * aInstanceScale, aGlobalPosition.z) + aInstancePosition, 1.0f);
}