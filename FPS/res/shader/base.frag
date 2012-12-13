#define FOG

uniform sampler2D tex;
uniform float highlight;

void doBasePre() {
#ifndef DEBUG
    if (!gl_FrontFacing)
        discard;
#endif
    gl_FragColor = texture2D(tex, gl_TexCoord[0].st);
    gl_FragColor.r *= highlight;
    gl_FragColor.g *= highlight;
    gl_FragColor.b *= highlight;
    //gl_FragColor.a = 1.0;
}

void doBasePost() {
#ifdef FOG
    float fog = (gl_Fog.end - gl_FogFragCoord)  / (gl_Fog.end - gl_Fog.start);
    fog = min(1.0, max(0.0, 1.0 - fog));
    gl_FragColor = mix(gl_FragColor, gl_Fog.color, fog);
#endif
}