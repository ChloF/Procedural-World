using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public int tickRate;
    public GameObject[] foodTypes;
    public float[] spawnRates;
    public int maxFood;
    public int maxSpawnPointTrials;
    public bool spawning;
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
			//Check if the spawner is enabled, and if it has already exceeded the max amont of food.
            if(spawning && Food.allFood.Count < maxFood)
            {
				//Iterate over each food type.
				for (int i = 0; i < foodTypes.Length; i++)
				{
					//Randomly choose whether to spawn, so the rate per second is equal to the corresponding spawnrate.
					if (Random.value < spawnRates[i] / tickRate)
					{
						SpawnFood(foodTypes[i]);
					}
				}
            }
			//Run the coroutine tickRate times per second.	
            yield return new WaitForSeconds(1 / tickRate);
        }
    }

    private void SpawnFood(GameObject food)
    {
		//Instantiate the food at a specific spawnPoint.
        Food f = food.GetComponent<Food>();
        Vector3 p = SpawnPoint(f.size / 2, f.minHeight, f.maxHeight);
            
        Instantiate(food, p, Quaternion.identity);
    }

    private Vector3 SpawnPoint(float heightOffset, float minHeight, float maxHeight)
    {
        //Only try to find a spawnpoint a fixed number of times.
        for(int i = 0; i < maxSpawnPointTrials; i++)
        {
            //Pick random x and y values.
            float tx = Random.value;
            float ty = Random.value;
            
            //Interpolate between the maximum and minimum.
            float x = Mathf.Lerp(xMin, xMax, tx);
            float y = Mathf.Lerp(yMin, yMax, ty);
            
            //Try and create a spawnPoint
            Vector3 spawnPoint = new Vector3(x, 100, y);

            Ray ray = new Ray(spawnPoint, Vector3.down);
            
            //If there is land under the spawnPoint, and that land is within the height range for that food,
            //set the spawnPoint to be at that point on the ground.
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            {
                if (hit.point.y >= minHeight && hit.point.y <= maxHeight)
                {
                    return hit.point + Vector3.up * heightOffset;
                }
            }
        }
        
        //If no spawn point can be found in a reasonable number of trials, give a warning.
        Debug.LogWarning("Could not find a valid food spawn point.");
        return Vector3.zero;
    }
}
