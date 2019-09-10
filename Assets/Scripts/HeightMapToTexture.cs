using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapToTexture
{ 
    public static Texture2D GenerateTexture(float[,] heightMap, TerrainType[,] regionMap, FilterMode filterMode)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        
        //Create an array to store all the colours in the texture.
        Color[] colourMap = new Color[(width - 1) * (height - 1)];

        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                //Add the colour from the region map to the colourMap array.
                colourMap[x + y * (width - 1)] = regionMap[x, y].colour;
            }
        }
        
        Texture2D tex = new Texture2D(width - 1, height - 1);

        tex.SetPixels(colourMap);
        tex.filterMode = filterMode;

        return tex;
    }
    
    //Creates map, stating which terrain region any given vertex is in.
    public static TerrainType[,] GenerateRegionMap(float[,] heightMap, TerrainType[] regions)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        TerrainType[,] regionMap = new TerrainType[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //Check which region a certain point is in.
                float curHeight = heightMap[x, y];

                for (int i = 0; i < regions.Length; i++)
                {
                    if (curHeight <= regions[i].height)
                    {
                        regionMap[x, y] = regions[i];
                        break;
                    }
                }
            }
        }

        return regionMap;
    }
}
