#version 330

in vec2 fragTexCoord;
in vec4 fragColor;

out vec4 finalColor;

uniform sampler2D texture0;

uniform float time;

// Intensidad horizontal de la ondulación
uniform float waveStrength;

// Cantidad de ondas verticales
uniform float waveFrequency;

// Tamaño del pixelado de la distorsión
uniform float pixelSize;

// Color que se mezcla con el agua
uniform vec4 waterColor;

// Intensidad del tinte
uniform float waterTint;

void main()
{
    vec2 uv = fragTexCoord;

    // ----------------------------------------
    // Pixelizar las coordenadas
    // ----------------------------------------

    vec2 pixelUV = floor(uv * pixelSize) / pixelSize;

    // ----------------------------------------
    // Onda horizontal
    // ----------------------------------------

    float wave =
        sin(
            pixelUV.y * waveFrequency +
            time * 2.0
        );

    // Segunda onda más pequeña
    float wave2 =
        sin(
            pixelUV.y * waveFrequency * 0.5 -
            time * 1.3
        );

    float offset =
        (wave * 0.7 + wave2 * 0.3)
        * waveStrength;

    // Pixelizar también el desplazamiento
    offset =
        floor(offset * pixelSize + 0.5)
        / pixelSize;

    uv.x += offset;

    // ----------------------------------------
    // Clamp
    // ----------------------------------------

    uv = clamp(uv, 0.0, 1.0);

    // ----------------------------------------
    // Textura
    // ----------------------------------------

    vec4 color =
        texture(texture0, uv);

    // ----------------------------------------
    // Tinte de agua
    // ----------------------------------------

    color.rgb =
        mix(
            color.rgb,
            color.rgb * waterColor.rgb,
            waterTint
        );

    finalColor =
        color * fragColor;
}
