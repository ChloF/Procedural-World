using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapToMesh
{
    public static Mesh GenerateMesh(float[,] heightMap, float heightScale, bool flatShading)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        List<Vector3> smoothVertices = new List<Vector3>();
        List<Vector3> flatVertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        int[] triangles = new int[(width - 1) * (height - 1) * 6];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                smoothVertices.Add(new Vector3(x, heightMap[x, y] * heightScale, y));

                if(!flatShading)
                {
                    uv.Add(new Vector2((float)x / (width - 1), (float)y / (width - 1)));
                }
            }
        }

        for (int tri = 0, vert = 0, y = 0; y < height-1; y++, vert++)
        {
            for (int x = 0; x < width-1; x++, tri += 6, vert++)
            {
                triangles[tri] = vert;
                triangles[tri + 3] = triangles[tri + 2] = vert + 1;
                triangles[tri + 4] = triangles[tri + 1] = vert + width;
                triangles[tri + 5] = vert + width + 1;
            }
        }

        Mesh mesh = new Mesh();

        if (flatShading)
        {
            for (int i = 0; i < triangles.Length; i++)
            {
                flatVertices.Add(smoothVertices[triangles[i]]);
                triangles[i] = i;

                Vector3 vertPos = flatVertices[i];

                uv.Add(new Vector2(vertPos.x / (width - 1), vertPos.z / (height - 1)));
            }

            mesh.SetVertices(flatVertices);
        }
        else
        {
            mesh.SetVertices(smoothVertices);
        }

        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uv);
        mesh.RecalculateNormals();

        return mesh;
    }
}
