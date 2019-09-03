using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ProceduralTerrain))]
[CanEditMultipleObjects]
public class ProceduralTerrainEditor : Editor
{
    ProceduralTerrain terrain;

    static bool displayNoiseProperties = false;
    static bool displayRenderingProperties = false;
    static bool displayRegions = false;

    private void OnEnable()
    {
        terrain = (ProceduralTerrain)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        terrain.width = Mathf.Clamp(EditorGUILayout.IntField("Width", terrain.width), 2, 100);
        terrain.depth = Mathf.Clamp(EditorGUILayout.IntField("Depth", terrain.depth), 2, 100);
        terrain.height = Mathf.Clamp(EditorGUILayout.FloatField("Height", terrain.height), 0, float.MaxValue);
        terrain.heightCurve = EditorGUILayout.CurveField("Height Curve", terrain.heightCurve);
        terrain.scale = Mathf.Clamp(EditorGUILayout.FloatField("Scale", terrain.scale), 0.1F, float.MaxValue);

        displayNoiseProperties = EditorGUILayout.BeginFoldoutHeaderGroup(displayNoiseProperties, "Noise");

        if (displayNoiseProperties)
        {
            terrain.noiseScale = EditorGUILayout.FloatField("Scale", terrain.noiseScale);
            terrain.noiseOctaves = EditorGUILayout.IntSlider("Octaves", terrain.noiseOctaves, 1, 10);
            terrain.noisePersistence = EditorGUILayout.Slider("Persistence", terrain.noisePersistence, 0, 1);
            terrain.noiseLacunarity = EditorGUILayout.Slider("Lacunarity", terrain.noiseLacunarity, 1, 3);
            terrain.noiseOffset = EditorGUILayout.Vector2Field("Offset", terrain.noiseOffset);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();

            terrain.seed = EditorGUILayout.IntField("Seed", terrain.seed);

            if (GUILayout.Button("Randomise Seed"))
            {
                terrain.seed = Mathf.RoundToInt(Random.Range(-100000, 100000));
            }

            GUILayout.EndHorizontal();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();


        displayRenderingProperties = EditorGUILayout.BeginFoldoutHeaderGroup(displayRenderingProperties, "Rendering");

        if (displayRenderingProperties)
        {
            terrain.flatShading = EditorGUILayout.Toggle("Use Flat Shading", terrain.flatShading);
            terrain.textureFilterMode = (FilterMode)EditorGUILayout.EnumPopup("Texture Filter Mode", terrain.textureFilterMode);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();


        displayRegions = EditorGUILayout.BeginFoldoutHeaderGroup(displayRegions, "Regions");

        if (displayRegions)
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(60)))
            {
                Material mat = new Material(Shader.Find("Standard"));
                terrain.regions.Add(new TerrainType("New Region", 1.0f, Color.black, mat));
            }

            GUILayout.Space(20);

            EditorGUILayout.EndHorizontal();

            List<TerrainType> modifiedRegionList = new List<TerrainType>();

            for (int i = 0; i < terrain.regions.Count; i++)
            {
                TerrainType modifiedRegion = new TerrainType();

                modifiedRegion.name = EditorGUILayout.TextField(terrain.regions[i].name, GUILayout.Width(100));

                EditorGUILayout.BeginHorizontal();

                modifiedRegion.height = EditorGUILayout.Slider(terrain.regions[i].height, 0, 1);
                modifiedRegion.colour = EditorGUILayout.ColorField(terrain.regions[i].colour);  
                modifiedRegion.material = (Material)EditorGUILayout.ObjectField(terrain.regions[i].material, typeof(Material));

                if (!GUILayout.Button("X", GUILayout.Width(30)))
                {
                    modifiedRegionList.Add(modifiedRegion);
                }

                EditorGUILayout.EndHorizontal();
            }

            terrain.regions = modifiedRegionList;

            GUILayout.Space(10);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();
        GUILayout.Button(terrain.texture, GUILayout.Width(terrain.texture.width), GUILayout.Height(terrain.texture.height));
        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        if(EditorGUI.EndChangeCheck())
        {
            terrain.OnValidate();
        }

    }
}
