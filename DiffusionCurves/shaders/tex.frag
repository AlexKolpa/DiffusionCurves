#version 150
 
precision highp float;

uniform sampler2D background;

in vData
{
	vec2 texture_coord;
}frag;

out vec4 out_frag_color;

void main(void)
{              
  out_frag_color = texture2D(background, frag.texture_coord);
}