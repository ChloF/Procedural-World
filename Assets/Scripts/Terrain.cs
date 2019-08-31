using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Terrain : MonoBehaviour
{
    public int width, height;
    public int seed;
    public Vector2 offset;
    public float heightScale;
    public float noiseScale;
    public int octaves;
    [Range(0,1)]
    public float persistence;
    public float lacunarity;
    public TerrainType[] regions;

    void Update()
    {
        float[,] heightMap = Noise.GenerateNoiseMap(width, height, noiseScale, octaves, persistence, lacunarity, seed, offset);

        Color[] colourMap = GenerateColourMap(heightMap);
        Mesh terrainMesh = HeightMapToMesh.GenerateMesh(heightMap, heightScale);

        GetComponent<MeshFilter>().mesh = terrainMesh;
    }

    public Color[] GenerateColourMap(float[,] heightMap)
    {
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

        return colourMap;
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}