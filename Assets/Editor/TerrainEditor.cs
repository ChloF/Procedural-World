﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


[CustomEditor(typeof(ProceduralTerrain))]
[CanEditMultipleObjects]
public class ProceduralTerrainEditor : Editor
{
    ProceduralTerrain terrain;

    bool displayNoiseProperties = false;
    bool displayRenderingProperties = false;
    bool displayRegions = false;

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

        NoiseInspector();
        RenderingInspector();
        RegionInspector();

        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();
        GUILayout.Button(terrain.texture, GUILayout.Width(terrain.texture.width), GUILayout.Height(terrain.texture.height));
        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {
            terrain.OnValidate();
        }
    }

    void NoiseInspector()
    {
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
    }

    void RenderingInspector()
    {
        displayRenderingProperties = EditorGUILayout.BeginFoldoutHeaderGroup(displayRenderingProperties, "Rendering");

        if (displayRenderingProperties)
        {
            terrain.flatShading = EditorGUILayout.Toggle("Use Flat Shading", terrain.flatShading);
            terrain.textureFilterMode = (FilterMode)EditorGUILayout.EnumPopup("Texture Filter Mode", terrain.textureFilterMode);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    void RegionInspector()
    {
        displayRegions = EditorGUILayout.BeginFoldoutHeaderGroup(displayRegions, "Regions");

        if (displayRegions)
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(60)))
            {
                terrain.regions.Add(new TerrainType());
            }

            if (GUILayout.Button("-", GUILayout.Width(60)))
            {
                terrain.regions.Remove(terrain.regions[terrain.regions.Count - 1]);
            }

            GUILayout.Space(20);

            EditorGUILayout.EndHorizontal();


            for (int i = 0; i < terrain.regions.Count; i++)
            {
                TerrainType modifiedRegion = new TerrainType
                {
                    name = EditorGUILayout.TextField(terrain.regions[i].name, GUILayout.Width(100))
                };

                EditorGUILayout.BeginHorizontal();

                modifiedRegion.height = EditorGUILayout.Slider(terrain.regions[i].height, 0, 1);
                modifiedRegion.colour = EditorGUILayout.ColorField(terrain.regions[i].colour);

                EditorGUILayout.EndHorizontal();

                terrain.regions[i] = modifiedRegion;
            }

            GUILayout.Space(10);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}
