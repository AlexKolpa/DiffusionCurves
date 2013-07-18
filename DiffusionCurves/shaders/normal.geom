#version 330 compatibility   

uniform int screen_diagonal;
uniform float ratio;

layout(lines_adjacency) in;
layout(triangle_strip, max_vertices=8) out;

in vData
{
	vec4 left_color;
	vec4 right_color;
}vertex[];

out fData
{
	vec4 color;
}frag;

vec4 getperpendicular(int index)
{
	vec4 leftDir = normalize(gl_in[index].gl_Position - gl_in[index - 1].gl_Position);
	vec4 rightDir = normalize(gl_in[index].gl_Position - gl_in[index + 1].gl_Position);

	vec4 tangent =rightDir - leftDir;
	return  normalize(vec4(tangent.y / ratio, -tangent.x * ratio, 0, 0)) * ratio * 2;
}

void main()
{	
		frag.color = vertex[1].left_color;
		gl_Position = gl_in[1].gl_Position;
		EmitVertex();
		frag.color = vertex[2].left_color;
		gl_Position = gl_in[2].gl_Position;
		EmitVertex();
		
		frag.color = vertex[1].left_color;
		gl_Position = gl_in[1].gl_Position + (getperpendicular(1) + vec4(0,0,1f,0)) * screen_diagonal;
		EmitVertex();
		frag.color = vertex[2].left_color;
		gl_Position = gl_in[2].gl_Position + (getperpendicular(2) + vec4(0,0,1f,0)) * screen_diagonal;
		EmitVertex();		
		EndPrimitive();

		frag.color = vertex[1].right_color;
		gl_Position = gl_in[1].gl_Position;
		EmitVertex();
		frag.color = vertex[2].right_color;
		gl_Position = gl_in[2].gl_Position;
		EmitVertex();

		frag.color = vertex[1].right_color;
		gl_Position = gl_in[1].gl_Position + (- getperpendicular(1) + vec4(0,0,1f,0)) * screen_diagonal;
		EmitVertex();		
		frag.color = vertex[2].right_color;
		gl_Position = gl_in[2].gl_Position + (- getperpendicular(2) + vec4(0,0,1f,0)) * screen_diagonal;
		EmitVertex();
		EndPrimitive();
}