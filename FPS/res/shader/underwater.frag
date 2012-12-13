void doBasePre();
void doBasePost();

void main(void) {
    doBasePre();
    gl_FragColor = mix(gl_FragColor, vec4(0.5, 0.5, 0.9, 1), 0.5);
    doBasePost();
}