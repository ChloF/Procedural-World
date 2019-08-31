using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(MeshFilter))]
public class ProceduralTerrain : MonoBehaviour
{
    public int width, height;
    public int seed;
    public Vector2 noiseOffset;
    public float heightScale;
    public AnimationCurve heightCurve;
    public float noiseScale;
    public int noiseOctaves;
    [Range(0,1)]
    public float noisePersistence;
    public float noiseLacunarity;
    public bool flatShading;
    public FilterMode textureFilterMode;
    public List<TerrainType> regions;
    public Texture2D texture;

    private MeshFilter filter;
    private MeshCollider col;
    private MeshRenderer rend;


    private void Start()
    {
        UpdateTerrain();
    }

    public void UpdateTerrain()
    {
        float[,] heightMap = Noise.GenerateNoiseMap(width, height, noiseScale, noiseOctaves, noisePersistence, noiseLacunarity, seed, noiseOffset);

        Mesh terrainMesh = HeightMapToMesh.GenerateMesh(heightMap, heightScale, heightCurve, flatShading);
        terrainMesh.name = "Terrain";
        texture = HeightMapToTexture.GenerateTexture(heightMap, regions.ToArray(), textureFilterMode);

        filter = GetComponent<MeshFilter>();
        col = GetComponent<MeshCollider>();
        rend = GetComponent<MeshRenderer>();

        filter.mesh = terrainMesh;
        col.sharedMesh = terrainMesh;
        rend.sharedMaterial.mainTexture = texture;
    }

    public void OnValidate()
    {
        UpdateTerrain();
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;

    public void SetName(string newName)
    {
        name = newName;
    }

    public void SetHeight(float newHeight)
    {
        height = newHeight;
    }

    public void SetColour(Color newColour)
    {
        colour = newColour;
    }
}