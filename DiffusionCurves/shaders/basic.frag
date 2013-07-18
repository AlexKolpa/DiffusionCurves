#version 150
 
precision highp float;

uniform vec4 pixel_color;

out vec4 out_frag_color;

void main(void)
{              
  out_frag_color = pixel_color;
}