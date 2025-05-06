#version 330 core

layout (location = 0) in vec3 aGlobalPosition;
layout (location = 1) in vec3 aInstancePosition;
layout (location = 2) in vec2 aInstanceScale;
layout (location = 3) in vec3 aInstanceColor;

layout (std140) uniform Uniforms {
    mat4 uProjection;
};

out vec3 vColor;

void main() {
    vColor = aInstanceColor;
    gl_Position = uProjection * vec4(vec3(aGlobalPosition.xy * aInstanceScale, aGlobalPosition.z) + aInstancePosition, 1.0f);
}