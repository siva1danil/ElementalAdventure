#version 330 core

uniform sampler2D uTexture;

in vec2 vUV;

out vec4 FragColor;

void main() {
    vec4 color = texture(uTexture, vUV);
    if (color.a == 0.0)
        discard;
    FragColor = color;
}