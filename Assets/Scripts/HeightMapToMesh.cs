using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapToMesh
{
    public static Mesh GenerateMesh(float[,] heightMap, TerrainType[,] regionMap, TerrainType[] regions, float heightScale, AnimationCurve heightCurve, bool flatShading)
    {
        //Get the width and height of the heightmap.
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        
        //Create the vertices differently depending on whether the mesh is flat or smooth.
        List<Vector3> vertices = flatShading ? GenerateVerticesFlat(heightMap, width, height, heightScale, heightCurve) : GenerateVerticesSmooth(heightMap, width, height, heightScale, heightCurve);
        List<Vector2> uv = GenerateUVs(vertices, width, height);
        int[] triangles = flatShading ? GenerateTrianglesFlat(vertices) : GenerateTrianglesSmooth(width, height);
        
        //Create one submesh for each region in the terrain.
        int subMeshCount = regions.Length;
        
        //Split mesh into submeshes based on region.
        List<int[]> subMeshTriangles = SeparateMesh(vertices, width, height, triangles, regionMap, regions);

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);

        mesh.subMeshCount = subMeshCount;
        
        //Add the triangles from each submesh to the mesh.
        for (int i = 0; i < subMeshCount; i++)
        {
            mesh.SetTriangles(subMeshTriangles[i], i);
        }

        mesh.SetUVs(0, uv);
        mesh.RecalculateNormals();

        return mesh;
    }

    private static List<Vector3> GenerateVerticesSmooth(float[,] heightMap, int width, int height, float heightScale, AnimationCurve heightCurve)
    {
        List<Vector3> verts = new List<Vector3>(width * height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //Get the height of each vertext from the Height Map.
                float vertexHeight = heightCurve.Evaluate(heightMap[x, y]) * heightScale;

                verts.Add(new Vector3(x, vertexHeight, y));
            }
        }

        return verts;
    }

    private static List<Vector3> GenerateVerticesFlat(float[,] heightMap, int width, int height, float heightScale, AnimationCurve heightCurve)
    {
        List<Vector3> verts = new List<Vector3>();

        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                //Add vertices in triangle order, ensures 3 unique vertices per triangle.
                verts.Add(new Vector3(x, heightCurve.Evaluate(heightMap[x, y]) * heightScale, y));
                verts.Add(new Vector3(x + 1, heightCurve.Evaluate(heightMap[x + 1, y + 1]) * heightScale, y + 1));
                verts.Add(new Vector3(x + 1, heightCurve.Evaluate(heightMap[x + 1, y]) * heightScale, y));

                verts.Add(new Vector3(x, heightCurve.Evaluate(heightMap[x, y]) * heightScale, y));
                verts.Add(new Vector3(x, heightCurve.Evaluate(heightMap[x, y + 1]) * heightScale, y + 1));
                verts.Add(new Vector3(x + 1, heightCurve.Evaluate(heightMap[x + 1, y + 1]) * heightScale, y + 1));
            }
        }

        return verts;
    }

    private static List<Vector2> GenerateUVs(List<Vector3> vertices, int width, int height)
    {
        List<Vector2> uv = new List<Vector2>(width * height);

        foreach (Vector3 v in vertices)
        {
            uv.Add(new Vector2(v.x / (width - 1), v.z / (height - 1)));
        }

        return uv;
    }

    private static int[] GenerateTrianglesSmooth(int width, int height)
    {
        List<int> triangles = new List<int>();

        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                triangles.Add(x + width * y);
                triangles.Add(x + 1 + width * (y + 1));
                triangles.Add(x + 1 + width * y);

                triangles.Add(x + width * y);
                triangles.Add(x + width * (y + 1));
                triangles.Add(x + 1 + width * (y + 1));
            }
        }

        return triangles.ToArray();
    }

    private static int[] GenerateTrianglesFlat(List<Vector3> vertices)
    {
        List<int> triangles = new List<int>();

        for (int i = 0; i < vertices.Count; i++)
        {
            triangles.Add(i);
        }

        return triangles.ToArray();
    }

    private static List<int[]> SeparateMesh(List<Vector3> vertices, int width, int height, int[] triangles, TerrainType[,] regionMap, TerrainType[] regions)
    {
        int regionCount = regions.Length;
        List<List<int>> subMeshTriangles = new List<List<int>>();
        for (int i = 0; i < regionCount; i++)
        {
            subMeshTriangles.Add(new List<int>());
        }

        for (int v = 0; v < triangles.Length; v+=6)
        {
            Vector3 vertex = vertices[triangles[v]];
            int subMeshIndex = Array.IndexOf(regions, regionMap[Mathf.RoundToInt(vertex.x), Mathf.RoundToInt(vertex.z)]);

            for (int i = 0; i < 6; i++)
            {
                subMeshTriangles[subMeshIndex].Add(triangles[v+i]);
            }
        }

        List<int[]> newTriangles = new List<int[]>();

        for (int i = 0; i < subMeshTriangles.Count; i++)
        {
            newTriangles.Add(subMeshTriangles[i].ToArray());
        }

        return newTriangles;
    }
}
