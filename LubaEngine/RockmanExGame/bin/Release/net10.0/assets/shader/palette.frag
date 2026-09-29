#version 330

in vec2 fragTexCoord;
in vec4 fragColor;

out vec4 finalColor;

uniform sampler2D texture0;
uniform sampler2D texture1;

uniform float originalPalette;
uniform float targetPalette;

uniform float paletteWidth;
uniform float paletteHeight;

void main()
{
    vec4 sourceColor =
        texture(texture0, fragTexCoord);

    vec4 resultColor =
        sourceColor;

    // No modificar transparencias
    if (sourceColor.a > 0.0)
    {
        for (int i = 0; i < 256; i++)
        {
            if (float(i) >= paletteWidth)
                break;

            vec2 originalUV = vec2(
                (float(i) + 0.5) / paletteWidth,
                (originalPalette + 0.5) / paletteHeight
            );

            vec4 originalColor =
                texture(texture1, originalUV);

            if (distance(
                    sourceColor.rgb,
                    originalColor.rgb
                ) < 0.001)
            {
                vec2 targetUV = vec2(
                    (float(i) + 0.5) / paletteWidth,
                    (targetPalette + 0.5) / paletteHeight
                );

                resultColor =
                    texture(texture1, targetUV);

                break;
            }
        }
    }

    finalColor =
        resultColor * fragColor;
}
