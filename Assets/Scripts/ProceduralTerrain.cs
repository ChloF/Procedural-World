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
        float t = Time.realtimeSinceStartup;

        float[,] heightMap = Noise.GenerateNoiseMap(width, depth, noiseScale, noiseOctaves, noisePersistence, noiseLacunarity, seed, noiseOffset);
        TerrainType[,] regionMap = HeightMapToTexture.GenerateRegionMap(heightMap, regions.ToArray());
        texture = HeightMapToTexture.GenerateTexture(heightMap, regionMap, textureFilterMode);
        Mesh terrainMesh = HeightMapToMesh.GenerateMesh(heightMap, regionMap, regions.ToArray(), height, heightCurve, flatShading);
        terrainMesh.name = "Terrain";

        filter = GetComponent<MeshFilter>();
        col = GetComponent<MeshCollider>();
        rend = GetComponent<MeshRenderer>();

        List<Material> mats = new List<Material>();
        for (int subMesh = 0; subMesh < terrainMesh.subMeshCount; subMesh++)
        {
            Material material = regions[subMesh].material;
            material.mainTexture = texture;
            mats.Add(material);
        }

        rend.sharedMaterials = mats.ToArray();
        filter.mesh = terrainMesh;
        col.sharedMesh = terrainMesh;

        transform.localScale = Vector3.one * scale;

        print("Terrain Generated in " + (Time.realtimeSinceStartup - t) + "s.");
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

    public TerrainType(string _name, float _height, Color _colour, Material _material)
    {
        name = _name;
        height = _height;
        colour = _colour;
        material = _material;
    }
}