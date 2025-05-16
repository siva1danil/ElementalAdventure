#version 330 core

uniform sampler2D uTexture;

in vec2 vUV;

out vec4 fColor;

float median(float r, float g, float b) {
    return max(min(r, g), min(max(r, g), b));
}

void main() {
    // TODO: move to uniform
    vec4 u_FillColor = vec4(1.0);
    vec4 u_OutlineColor = vec4(0.0, 0.0, 0.0, 1.0);
    float u_OutlineWidth = 0.4;
    float u_Smoothing = 0.1;

    vec3 sdf = texture(uTexture, vUV).rgb;
    float dist = median(sdf.r, sdf.g, sdf.b) - 0.5;

    float fillAlpha = smoothstep(-u_Smoothing, u_Smoothing, dist);
    float outlineAlpha = smoothstep(-u_OutlineWidth - u_Smoothing, -u_OutlineWidth + u_Smoothing, dist);

    vec3 color = mix(u_OutlineColor.rgb, u_FillColor.rgb, fillAlpha);
    float alpha = max(outlineAlpha, fillAlpha);

    fColor = vec4(color, alpha);
}