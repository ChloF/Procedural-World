using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int width, int height, float scale, int octaves, float persistence, float lacunarity, int seed, Vector2 offset)
    {
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);

            octaveOffsets[i] = new Vector2(offsetX, offsetY) + offset;
        }

        float[,] noiseMap = new float[width, height];

        float min = float.MaxValue;
        float max = float.MinValue;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
             
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - width / 2) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - height / 2) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = 2 * Mathf.PerlinNoise(sampleX + seed, sampleY + seed) - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (noiseHeight < min)
                {
                    min = noiseHeight;
                }
                else if (noiseHeight > max)
                {
                    max = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        //Normalize
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(min, max, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
