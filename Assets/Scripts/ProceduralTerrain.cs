using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(MeshFilter))]
public class ProceduralTerrain : MonoBehaviour
{
    public int width, depth;
    public float scale;
    public int seed;
    public Vector2 noiseOffset;
    public float height;
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
        float[,] heightMap = Noise.GenerateNoiseMap(width, depth, noiseScale, noiseOctaves, noisePersistence, noiseLacunarity, seed, noiseOffset);

        Mesh terrainMesh = HeightMapToMesh.GenerateMesh(heightMap, height, heightCurve, flatShading);
        terrainMesh.name = "Terrain";
        texture = HeightMapToTexture.GenerateTexture(heightMap, regions.ToArray(), textureFilterMode);

        Material mat = new Material(Shader.Find("Standard"));
        mat.mainTexture = texture;
        mat.SetFloat("_Glossiness", 0.1F);
        mat.SetFloat("_Metallic", 0.0F);

        filter = GetComponent<MeshFilter>();
        col = GetComponent<MeshCollider>();
        rend = GetComponent<MeshRenderer>();

        filter.mesh = terrainMesh;
        col.sharedMesh = terrainMesh;
        rend.sharedMaterial = mat;

        transform.localScale = Vector3.one * scale;
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
    public Material material;
}