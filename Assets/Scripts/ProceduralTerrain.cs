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
    public AnimationCurve heightCurve; //Defines how the heightmap affects the height of the geometry.
    public float noiseScale;
    public int noiseOctaves;
    [Range(0,1)]
    public float noisePersistence;
    public float noiseLacunarity;
    public bool flatShading; //Toggles whether to use smooth or flat shading.
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
      
    public void OnValidate()
    {
        UpdateTerrain();
    }

    public void UpdateTerrain()
    {
        //Keep track of how long the function takes to run.
        float t = Time.realtimeSinceStartup;
        
        float[,] heightMap = Noise.GenerateNoiseMap(width, depth, noiseScale, noiseOctaves, noisePersistence, noiseLacunarity, seed, noiseOffset);
        TerrainType[,] regionMap = HeightMapToTexture.GenerateRegionMap(heightMap, regions.ToArray());
        texture = HeightMapToTexture.GenerateTexture(heightMap, regionMap, textureFilterMode);
        Mesh terrainMesh = HeightMapToMesh.GenerateMesh(heightMap, regionMap, regions.ToArray(), height, heightCurve, flatShading);
        terrainMesh.name = "Terrain";

        filter = GetComponent<MeshFilter>();
        col = GetComponent<MeshCollider>();
        rend = GetComponent<MeshRenderer>();
        
        //Set the material for each submesh from the list of regions.
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

        transform.localScale = Vector3.one * scale; //(scale, scale, scale)

        print("Terrain Generated in " + (Time.realtimeSinceStartup - t) + "s.");
    }
}

//Struct for each type of terrain.
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
