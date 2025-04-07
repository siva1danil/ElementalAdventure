#version 430 core

uniform sampler2D uTexture;

in vec3 vColor;
in vec2 vUV;

out vec4 FragColor;

void main() {
    FragColor = texture(uTexture, vec2(1.0f) - vUV) * vec4(vColor, 1.0f);
}