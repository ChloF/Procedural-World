using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int width, int height, float scale, int octaves, float persistence, float lacunarity, int seed, Vector2 offset)
    {
        //Initialise random generator.
        System.Random prng = new System.Random(seed);
        
        //Create random offsets to sample each octave from a different region.
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);

            octaveOffsets[i] = new Vector2(offsetX, offsetY) + offset;
        }
        
        float[,] noiseMap = new float[width, height];
        
        //Keep track of minimum and maximum for normalisation.
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
                    //Pick a point to sample perlin noise from.
                    float sampleX = (x - width / 2) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - height / 2) / scale * frequency + octaveOffsets[i].y;
                    
                    //Shift noise between -1/2 and 1/2.
                    float perlinValue = 2 * Mathf.PerlinNoise(sampleX + seed, sampleY + seed) - 1;
                    noiseHeight += perlinValue * amplitude;
                    
                    //Modify amplitude and frequency for each octave.
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }
                
                //Update minimum and maximum.
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

        //Normalise.
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //InverseLerp returns how far between min and max the noise value is.
                noiseMap[x, y] = Mathf.InverseLerp(min, max, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
