#version 330 core

// Interpolated values from the vertex shaders
in vec2 UV;
in vec4 VColor;

// Ouput data
out vec4 color;

void main(){
    // Output color = color of the texture at the specified UV

	color = VColor;
}