#version 330 core

uniform sampler2D uTexture;

in vec3 vColor;
in vec2 vUV;
flat in int vHasTexture;

out vec4 fColor;

void main() {
    vec4 color = vec4(vColor, 1.0) * mix(vec4(1.0), texture(uTexture, vUV), float(vHasTexture));
    if (color.a == 0.0)
        discard;
    fColor = color;
}