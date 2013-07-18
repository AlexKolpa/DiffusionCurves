#version 150

// incoming vertex position
in vec3 vertex_position;

uniform mat4 modelview_matrix;            
uniform mat4 projection_matrix;

//simply pass everything to the geometry shader
void main(void)
{          	    
	gl_Position = projection_matrix * modelview_matrix * vec4(vertex_position,1);
}