using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayNoise : MonoBehaviour
{
    public float[] colourThresholds;
    public Color[] colours;

    public Renderer rend;

    public void DisplayNoiseMap(float[,] noiseMap, Color[] colourMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;

        texture.SetPixels(colourMap);
        texture.Apply();

        rend.sharedMaterial.mainTexture = texture;
        rend.transform.localScale = new Vector3(width, 1, height);
    }
}
