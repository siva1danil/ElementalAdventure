#version 430 core

layout (location = 0) in vec3 aPosition;
layout (std430, binding = 0) buffer InstanceBuffer {
    struct {
        vec3 position;
        vec3 color;
    } instances[];
};

out vec3 vColor;

void main() {
    vColor = instances[gl_InstanceID].color;
    gl_Position = vec4(aPosition + instances[gl_InstanceID].position, 1.0f);
}