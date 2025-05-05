#version 330 core

uniform sampler2D uTexture;

in vec2 vUV;

out vec4 fColor;

void main() {
    vec4 color = texture(uTexture, vUV);
    if (color.a == 0.0)
        discard;
    fColor = color;
}