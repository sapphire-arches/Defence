#define FOG

void main(void) {
#ifndef DEBUG
    if (!gl_FrontFacing)
        discard;
#endif
    gl_FragColor = gl_Color;
    gl_FragColor.a = 0.5;
#ifdef FOG
    float fog = (gl_Fog.end - gl_FogFragCoord)  / (gl_Fog.end - gl_Fog.start);
    fog = min(1.0, max(0.0, 1.0 - fog));
    gl_FragColor = mix(gl_FragColor, gl_Fog.color, fog);
#endif
}