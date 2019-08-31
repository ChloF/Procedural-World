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

       
        Mesh terrainMesh = HeightMapToMesh.GenerateMesh(heightMap, heightScale);
        Texture2D texture = HeightMapToTexture.GenerateTexture(heightMap, regions);

        GetComponent<MeshFilter>().mesh = terrainMesh;
        GetComponent<MeshRenderer>().material.mainTexture = texture;
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}