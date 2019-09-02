using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapToTexture
{
    public static Texture2D GenerateTexture(float[,] heightMap, TerrainType[] regions, FilterMode filterMode)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colourMap = new Color[(width - 1) * (height - 1)];

        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                float curHeight = heightMap[x, y];

                for (int i = 0; i < regions.Length; i++)
                {
                    if (curHeight <= regions[i].height)
                    {
                        colourMap[y * (width - 1) + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }


        Texture2D tex = new Texture2D(width - 1, height - 1);

        tex.SetPixels(colourMap);
        tex.filterMode = filterMode;

        return tex;
    }
}
