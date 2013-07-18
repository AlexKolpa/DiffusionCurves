#version 150

// incoming vertex position
in vec3 vertex_position;
in vec2 tex_coord;

uniform mat4 modelview_matrix;            
uniform mat4 projection_matrix;

out vData
{
	vec2 texture_coord;
}vertex;

//simply pass everything to the geometry shader
void main(void)
{              
	vertex.texture_coord = tex_coord;
	gl_Position = projection_matrix * modelview_matrix * vec4(vertex_position,1);
}
