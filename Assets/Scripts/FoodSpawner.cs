using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public int tickRate;
    public GameObject[] foodTypes;
    public float[] spawnRates;
    public int maxSpawnPointTrials;
    public int maxFood;
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while(true)
        {
            if (Food.allFood.Count < maxFood)
            {
                for (int i = 0; i < foodTypes.Length; i++)
                {
                    if (Random.value < spawnRates[i] / tickRate)
                    {
                        SpawnFood(foodTypes[i]);
                    }
                }
            }

            yield return new WaitForSeconds(1 / tickRate);
        }
    }

    private void SpawnFood(GameObject food)
    {
        Food f = food.GetComponent<Food>();
        Vector3 p = SpawnPoint(f.size / 2, f.minHeight, f.maxHeight);
            
        Instantiate(food, p, Quaternion.identity);
    }

    private Vector3 SpawnPoint(float heightOffset, float minHeight, float maxHeight)
    {
        int i = 0;
        while (true)
        {
            float tx = Random.value;
            float ty = Random.value;

            float x = Mathf.Lerp(xMin, xMax, tx);
            float y = Mathf.Lerp(yMin, yMax, ty);

            Vector3 spawnPoint = new Vector3(x, 100, y);

            Ray ray = new Ray(spawnPoint, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            {
                if (hit.point.y >= minHeight && hit.point.y <= maxHeight)
                {
                    return hit.point + Vector3.up * heightOffset;
                }
            }

            i++;

            if(i > 50)
            {
                Debug.LogError("Could not find a valid food spawn point.");
                return Vector3.zero;
            }
        }
    }
}
