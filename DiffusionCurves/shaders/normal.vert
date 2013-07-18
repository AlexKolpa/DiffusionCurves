#version 150

// incoming vertex position
in vec3 vertex_position;
in vec4 left_color;
in vec4 right_color;

uniform mat4 modelview_matrix;            
uniform mat4 projection_matrix;

out vData
{
	vec3 normal;
	vec4 left_color;
	vec4 right_color;
}vertex;

//simply pass everything to the geometry shader
void main(void)
{          
	vertex.left_color = left_color;
	vertex.right_color = right_color;
	gl_Position = projection_matrix * modelview_matrix * vec4(vertex_position,1);
}
