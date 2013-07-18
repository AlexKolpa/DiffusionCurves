#version 150
 
precision highp float;

uniform sampler2D color_texture;
uniform sampler2D depth_texture;

uniform int iteration;
uniform float xScale;
uniform float yScale;
in vData
{
	vec2 texture_coord;
}frag;

out vec4 out_frag_color;

void main(void)
{              

	float offset = 0.92387 * texture2D(depth_texture, frag.texture_coord).x / iteration;
	vec4 accum = vec4(0,0,0,0);
	accum += texture2D(color_texture, frag.texture_coord + vec2(xScale * offset, 0))/4f;
	accum += texture2D(color_texture, frag.texture_coord + vec2(xScale * -offset, 0))/4f;
	accum += texture2D(color_texture, frag.texture_coord + vec2(0, yScale * offset))/4f;
	accum += texture2D(color_texture, frag.texture_coord + vec2(0, yScale * -offset))/4f;

	out_frag_color = accum;
}