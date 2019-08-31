using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapToTexture
{
    public static Texture2D GenerateTexture(float[,] heightMap, TerrainType[] regions, FilterMode filterMode)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colourMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float curHeight = heightMap[x, y];

                for (int i = 0; i < regions.Length; i++)
                {
                    if (curHeight <= regions[i].height)
                    {
                        colourMap[y * width + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }


        Texture2D tex = new Texture2D(width, height);

        tex.SetPixels(colourMap);
        tex.filterMode = filterMode;

        return tex;
    }
}
