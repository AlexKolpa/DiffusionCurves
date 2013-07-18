#version 150
 
precision highp float;

in fData
{
	vec4 color;
}frag;

out vec4 out_frag_color;

void main(void)
{              
  out_frag_color = frag.color;
}
