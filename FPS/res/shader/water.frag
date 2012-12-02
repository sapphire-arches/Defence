void doBasePre();
void doBasePost();

void main(void) {
    doBasePre();
    gl_FragColor.a = 0.5;
    doBasePost();
}