uniform mat4 modelview;
uniform mat4 projection;

void main (void) {
    gl_Position = projection * modelview * gl_Vertex;
    vec4 c = vec4(0, 0, 0, 1);
    c.r = abs(gl_Normal.x);
    c.g = abs(gl_Normal.y);
    c.b = abs(gl_Normal.z);
    gl_FrontColor = gl_Color;
    gl_BackColor = vec4(1, 0, 1, 0);
    gl_FogFragCoord = gl_Position.z;
}