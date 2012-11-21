uniform mat4 modelview;
uniform mat4 projection;

void main (void) {
    gl_Position = projection * modelview * gl_Vertex;
    gl_FrontColor = gl_Color; //gl_Vertex / 10.0;
    //gl_FrontColor.g *= 10f;
    gl_BackColor = vec4(1, 0, 1, 0);
    gl_FogFragCoord = gl_Position.z;
}