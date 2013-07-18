#version 330 compatibility   

layout(lines_adjacency) in;
layout(triangle_strip, max_vertices=100) out;

uniform int screen_diagonal;
uniform float ratio;

float M_PI = 3.1415926535897932384626433832795f;
float tesselation = 20.0f;

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

	vec4 tangent = rightDir - leftDir;
	return normalize(vec4(tangent.y / ratio, -tangent.x * ratio, 0, 0));
}

void triangle(vec3 normal, vec4 position, vec4 firstcolor, vec4 secondcolor)
{

	gl_Position = position;
	frag.color = firstcolor;
	EmitVertex();

	for(int j = 0; j < tesselation + 1; j++)
	{
		vec4 lerpedColor = mix(firstcolor, secondcolor, j/tesselation);

		float cosA = cos(j/tesselation * M_PI);
		float sinA = sin(j/tesselation * M_PI);
		
		gl_Position = position + vec4((normal.x*cosA - normal.y*sinA), 
		(normal.x*sinA + normal.y*cosA), 1, 0) * screen_diagonal;
		frag.color = lerpedColor;
		EmitVertex();	

		gl_Position = position;
		frag.color = lerpedColor;
		EmitVertex();
	}

	EndPrimitive();
}

void main()
{	
	vec3 leftPerp = getperpendicular(1).xyz;
	triangle(leftPerp * ratio * 2, gl_in[1].gl_Position, vertex[1].left_color, vertex[1].right_color);
}