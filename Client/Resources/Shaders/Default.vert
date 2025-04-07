#version 430 core

layout (location = 0) in vec3 aPosition;
layout (std430, binding = 0) buffer InstanceBuffer {
    struct {
        vec3 position;
        vec3 color;
    } aInstances[];
};

uniform mat4 uProjection = mat4(1.0f);

out vec3 vColor;
out vec2 vUV;

void main() {
    vColor = aInstances[gl_InstanceID].color;
    vUV = aPosition.xy;
    gl_Position = uProjection * vec4(aPosition + aInstances[gl_InstanceID].position, 1.0f);
}