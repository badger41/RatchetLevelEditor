#version 330 core

// Interpolated values from the vertex shaders
in vec2 UV;
in vec4 VColor;

// Ouput data
out vec3 color;

// Values that stay constant for the whole mesh.
uniform sampler2D myTextureSampler;
uniform vec4 replacementColor;

void main(){

    // Output color = color of the texture at the specified UV

	vec3 vcol = mix(vec3(1,1,1), VColor.rgb, VColor.a);
	color = texture( myTextureSampler, UV ).rgb * vcol.rgb;

	color = mix(color, replacementColor.rgb, replacementColor.a);
}